namespace Arktos.WinBert.UnitTests
{
    using System;
    using Arktos.WinBert.Analysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BertBehavioralAnalyzerUnitTests
    {
        #region Test Methods

        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullArgument()
        {
            var target = new BertBehavioralAnalyzer(null);
        }

        #endregion

        #endregion
    }
}
