namespace Arktos.WinBert.UnitTests.RandoopIntegration
{
    using Arktos.WinBert.Instrumentation;
    using Arktos.WinBert.RandoopIntegration;
    using Microsoft.Cci;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    [DeploymentItem(@"test-assembly-files\", @"test-assembly-files\")]
    public class RandoopTestRewriterUnitTests
    {
        #region Fields & Constants

        private static readonly IMetadataHost Host = new PeReader.DefaultHost();

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
        public void Rewrite_NullAssemblyTarget()
        {
            var target = RandoopTestRewriter.Create("HelloWorld", Host);
            IInstrumentationTarget toRewrite = null;
            var expected = target.Rewrite(toRewrite);
        }

        #endregion

        #endregion
    }
}
