namespace Arktos.WinBert.UnitTests
{
    using System;
    using Arktos.WinBert.Instrumentation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using AppDomainToolkit;
    using Moq;
    using System.Reflection;

    [TestClass]
    [DeploymentItem(@"test-assembly-files\", @"test-assembly-files\")]
    public class InstrumentationTargetUnitTests
    {
        #region Fields & Constants

        private static readonly string TestAssemblyDir = @"test-assembly-files\";

        private static readonly string NoRefsAssemblyName = @"TestWithNoReferences.dll";

        private static readonly string NoRefsAssemblyPath = Path.Combine(TestAssemblyDir, NoRefsAssemblyName);

        private static readonly string DiffTestAssembly1Name = @"DiffTestAssembly1.dll";

        private static readonly string DiffTestAssembly1Path = Path.Combine(TestAssemblyDir, DiffTestAssembly1Name);

        #endregion

        #region Test Methods

        #region Create

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Create_NullArgument()
        {
            var target = InstrumentationTarget.Create(null);
        }

        [TestMethod]
        public void Create_ValidAssembly()
        {
            var assembly = Assembly.LoadFile(Path.GetFullPath(NoRefsAssemblyPath));

            var mock = new Mock<IAssemblyTarget>();
            mock.Setup(x => x.Location).Returns(assembly.Location);
            mock.Setup(x => x.CodeBase).Returns(new Uri(assembly.CodeBase));
            mock.Setup(x => x.FullName).Returns(assembly.FullName);

            var target = InstrumentationTarget.Create(mock.Object);

            Assert.IsNotNull(target);
            Assert.IsNotNull(target.Host);
            Assert.IsNotNull(target.MutableAssembly);
            Assert.IsNotNull(target.PdbReader);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Create_InvalidPath()
        {
            var assembly = Assembly.LoadFile(Path.GetFullPath(NoRefsAssemblyPath));
            var fakePath = Path.GetFullPath(Path.Combine(TestAssemblyDir, Guid.NewGuid().ToString(), NoRefsAssemblyName));

            var mock = new Mock<IAssemblyTarget>();
            mock.Setup(x => x.Location).Returns(fakePath);
            mock.Setup(x => x.CodeBase).Returns(new Uri(fakePath));
            mock.Setup(x => x.FullName).Returns(assembly.FullName);

            var target = InstrumentationTarget.Create(mock.Object);

            Assert.IsNotNull(target);
            Assert.IsNotNull(target.Host);
            Assert.IsNotNull(target.MutableAssembly);
            Assert.IsNotNull(target.PdbReader);
        }

        #endregion

        #region Dispose

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Dispose_HostProperty()
        {
            var assembly = Assembly.LoadFile(Path.GetFullPath(NoRefsAssemblyPath));

            var mock = new Mock<IAssemblyTarget>();
            mock.Setup(x => x.Location).Returns(assembly.Location);
            mock.Setup(x => x.CodeBase).Returns(new Uri(assembly.CodeBase));
            mock.Setup(x => x.FullName).Returns(assembly.FullName);

            InstrumentationTarget target;
            using (target = InstrumentationTarget.Create(mock.Object))
            {
                Assert.IsNotNull(target);
                Assert.IsNotNull(target.Host);
                Assert.IsNotNull(target.MutableAssembly);
                Assert.IsNotNull(target.PdbReader);
            }

            Assert.IsTrue(target.IsDisposed);
            var host = target.Host;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Dispose_MutableModuleProperty()
        {
            var assembly = Assembly.LoadFile(Path.GetFullPath(NoRefsAssemblyPath));

            var mock = new Mock<IAssemblyTarget>();
            mock.Setup(x => x.Location).Returns(assembly.Location);
            mock.Setup(x => x.CodeBase).Returns(new Uri(assembly.CodeBase));
            mock.Setup(x => x.FullName).Returns(assembly.FullName);

            InstrumentationTarget target;
            using (target = InstrumentationTarget.Create(mock.Object))
            {
                Assert.IsNotNull(target);
                Assert.IsNotNull(target.Host);
                Assert.IsNotNull(target.MutableAssembly);
                Assert.IsNotNull(target.PdbReader);
            }

            Assert.IsTrue(target.IsDisposed);
            var module = target.MutableAssembly;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Dispose_PdbReaderProperty()
        {
            var assembly = Assembly.LoadFile(Path.GetFullPath(NoRefsAssemblyPath));

            var mock = new Mock<IAssemblyTarget>();
            mock.Setup(x => x.Location).Returns(assembly.Location);
            mock.Setup(x => x.CodeBase).Returns(new Uri(assembly.CodeBase));
            mock.Setup(x => x.FullName).Returns(assembly.FullName);

            InstrumentationTarget target;
            using (target = InstrumentationTarget.Create(mock.Object))
            {
                Assert.IsNotNull(target);
                Assert.IsNotNull(target.Host);
                Assert.IsNotNull(target.MutableAssembly);
                Assert.IsNotNull(target.PdbReader);
            }

            Assert.IsTrue(target.IsDisposed);
            var reader = target.PdbReader;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Dispose_Save()
        {
            var assembly = Assembly.LoadFile(Path.GetFullPath(NoRefsAssemblyPath));

            var mock = new Mock<IAssemblyTarget>();
            mock.Setup(x => x.Location).Returns(assembly.Location);
            mock.Setup(x => x.CodeBase).Returns(new Uri(assembly.CodeBase));
            mock.Setup(x => x.FullName).Returns(assembly.FullName);

            InstrumentationTarget target;
            using (target = InstrumentationTarget.Create(mock.Object))
            {
                Assert.IsNotNull(target);
                Assert.IsNotNull(target.Host);
                Assert.IsNotNull(target.MutableAssembly);
                Assert.IsNotNull(target.PdbReader);
            }

            Assert.IsTrue(target.IsDisposed);
            target.Save();
        }

        [TestMethod]
        public void Dispose_WithUsing()
        {
            var assembly = Assembly.LoadFile(Path.GetFullPath(NoRefsAssemblyPath));

            var mock = new Mock<IAssemblyTarget>();
            mock.Setup(x => x.Location).Returns(assembly.Location);
            mock.Setup(x => x.CodeBase).Returns(new Uri(assembly.CodeBase));
            mock.Setup(x => x.FullName).Returns(assembly.FullName);

            InstrumentationTarget target;
            using (target = InstrumentationTarget.Create(mock.Object))
            {
                Assert.IsNotNull(target);
                Assert.IsNotNull(target.Host);
                Assert.IsNotNull(target.MutableAssembly);
                Assert.IsNotNull(target.PdbReader);
            }

            Assert.IsTrue(target.IsDisposed);
        }

        #endregion

        #region Save

        [TestMethod]
        public void Save_WithPdb()
        {
            var assembly = Assembly.LoadFile(Path.GetFullPath(NoRefsAssemblyPath));

            var mock = new Mock<IAssemblyTarget>();
            mock.Setup(x => x.Location).Returns(assembly.Location);
            mock.Setup(x => x.CodeBase).Returns(new Uri(assembly.CodeBase));
            mock.Setup(x => x.FullName).Returns(assembly.FullName);

            var target = InstrumentationTarget.Create(mock.Object);

            Assert.IsNotNull(target);
            Assert.IsNotNull(target.Host);
            Assert.IsNotNull(target.MutableAssembly);
            Assert.IsNotNull(target.PdbReader);

            var actual = target.Save();
            Assert.IsTrue(File.Exists(actual.Location));
            Assert.IsTrue(File.Exists(actual.CodeBase.LocalPath));

            var modifiedAssembly = Assembly.LoadFile(actual.Location);
            Assert.AreEqual(modifiedAssembly.FullName, actual.FullName);
        }

        [TestMethod]
        public void Save_WithoutPdb()
        {
            var assembly = Assembly.LoadFile(Path.GetFullPath(DiffTestAssembly1Path));

            var mock = new Mock<IAssemblyTarget>();
            mock.Setup(x => x.Location).Returns(assembly.Location);
            mock.Setup(x => x.CodeBase).Returns(new Uri(assembly.CodeBase));
            mock.Setup(x => x.FullName).Returns(assembly.FullName);

            var target = InstrumentationTarget.Create(mock.Object);

            Assert.IsNotNull(target);
            Assert.IsNotNull(target.Host);
            Assert.IsNotNull(target.MutableAssembly);
            Assert.IsNull(target.PdbReader);

            var actual = target.Save();
            Assert.IsTrue(File.Exists(actual.Location));
            Assert.IsTrue(File.Exists(actual.CodeBase.LocalPath));

            var modifiedAssembly = Assembly.LoadFile(actual.Location);
            Assert.AreEqual(modifiedAssembly.FullName, actual.FullName);
        }

        #endregion

        #endregion
    }
}
