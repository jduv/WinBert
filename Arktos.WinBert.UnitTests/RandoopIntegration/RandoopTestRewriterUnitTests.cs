namespace Arktos.WinBert.UnitTests.RandoopIntegration
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Arktos.WinBert.RandoopIntegration;

    [TestClass]
    public class RandoopTestRewriterUnitTests
    {
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

        #endregion
    }
}
