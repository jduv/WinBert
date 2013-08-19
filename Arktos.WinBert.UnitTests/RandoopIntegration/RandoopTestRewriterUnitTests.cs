namespace Arktos.WinBert.UnitTests.RandoopIntegration
{
    using Arktos.WinBert.Instrumentation;
    using Arktos.WinBert.RandoopIntegration;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.IO;

    [TestClass]
    [DeploymentItem(@"test-assembly-files\", @"test-assembly-files\")]
    public class RandoopTestRewriterUnitTests
    {
        #region Fields & Constants

        private static readonly IMetadataHost Host = new PeReader.DefaultHost();

        private static readonly string TestDir = @"test-assembly-files\";

        private static readonly string TestAssemblyName = "RandoopTestRewriterTestAssembly.dll";

        private static readonly string TestAssemblyPath = TestDir + TestAssemblyName;

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
            //var mutableAssembly = this.LoadAssemblyForInstrumentation(TestAssemblyPath);
            var toInstrument = new Mock<IInstrumentationTarget>();
            //toInstrument.Setup(x => x.MutableAssembly).Returns(mutableAssembly);
            // Execute save once.
            toInstrument.Verify(x => x.Save(), Times.Once());

            var target = RandoopTestRewriter.Create("Main", Host);
            target.Rewrite(toInstrument.Object);
            Assert.Fail("Not Implemented");
        }

        [TestMethod]
        public void Rewrite_MethodBody()
        {
            Assert.Fail("Not Implemented");
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads an assembly from the target path and returns it as a mutable assembly
        /// in the CCI metadata sense.
        /// </summary>
        /// <param name="path">
        /// The path to the assembly to load. Cannot be null or empty.
        /// </param>
        /// <returns>
        /// A mutable assembly, copied by a metadata deep copier.
        /// </returns>
        private Assembly LoadAssemblyForInstrumentation(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Assembly path cannot be null or empty.");
            }

            var copier = new MetadataDeepCopier(Host);
            var assembly = Host.LoadUnitFrom(path) as Assembly;

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
