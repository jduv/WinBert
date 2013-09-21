namespace Arktos.WinBert.UnitTests.RandoopIntegration
{
    using AppDomainToolkit;
    using Arktos.WinBert.Instrumentation;
    using Arktos.WinBert.RandoopIntegration;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.IO;
    using System.Linq;

    [TestClass]
    [DeploymentItem(@"test-assembly-files\", @"test-assembly-files\")]
    public class RandoopTestRewriterUnitTests
    {
        #region Fields & Constants

        private static readonly IMetadataHost Host = new PeReader.DefaultHost();

        private static readonly string TestDir = @"test-assembly-files\";

        private static readonly string TestAssemblyName = "RandoopTestRewriterTestAssembly.dll";

        private static readonly string TestAssemblyPath = TestDir + TestAssemblyName;

        private static readonly string TestMethodName = "Main";

        #endregion

        #region Test Methods

        #region Create

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Create_NullMethodName()
        {
            var target = RandoopTestRewriter.Create(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Create_NullMetadataHost()
        {
            var target = RandoopTestRewriter.Create("HelloWorld", null);
        }

        #endregion

        #region Rewrite

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Rewrite_NullInstrumentationTarget()
        {
            var target = RandoopTestRewriter.Create("HelloWorld", Host);
            IInstrumentationTarget toRewrite = null;
            var expected = target.Rewrite(toRewrite);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Rewrite_NullMethodBody()
        {
            var target = RandoopTestRewriter.Create("HelloWorld", Host);
            IMethodBody toRewrite = null;
            var expected = target.Rewrite(toRewrite);
        }

        [TestMethod]
        public void Rewrite_InstrumentationTarget()
        {
            // Load assembly twice so we can compare them later.
            var originalAssembly = LoadMutableAssemblyForInstrumentation(TestAssemblyPath);
            var mutableAssembly = LoadMutableAssemblyForInstrumentation(TestAssemblyPath);

            // Set up mocks
            var toInstrument = new Mock<IInstrumentationTarget>();
            toInstrument.Setup(x => x.MutableAssembly).Returns(mutableAssembly);

            var target = RandoopTestRewriter.Create(TestMethodName, Host);
            var actual = target.Rewrite(toInstrument.Object);

            // Should have executed save once.
            toInstrument.Verify(x => x.Save(), Times.Once());

            Assert.Fail("Not Complete");
        }

        [TestMethod]
        public void Rewrite_MethodBody()
        {
            Assert.Fail("Not Implemented");
        }

        #endregion

        #endregion

        #region Private Methods

        private static Assembly LoadMutableAssemblyForInstrumentation(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Assembly path cannot be null or empty.");
            }

            var copier = new MetadataDeepCopier(Host);
            var assembly = Host.LoadUnitFrom(path) as IAssembly;

            // File not found. Couldn't load.
            if (assembly == null)
            {
                throw new FileNotFoundException(path);
            }

            return copier.Copy(assembly);
        }

        #endregion
    }
}
