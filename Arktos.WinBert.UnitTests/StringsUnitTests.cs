namespace Arktos.WinBert.UnitTests
{
    using Arktos.WinBert.Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StringsUnitTests
    {
        #region Test Methods

        #region IfEmptyThenNull

        [TestMethod]
        public void IfEmptyThenNull_CompleteTest()
        {
            var expected = Strings.IfEmptyThenNull((string)null);
            Assert.IsNull(expected);

            expected = Strings.IfEmptyThenNull(string.Empty);
            Assert.IsNull(expected);

            expected = Strings.IfEmptyThenNull("Hello World");
            Assert.IsNotNull(expected);
        }

        #endregion

        #region PrettyPrintNullOrEmpty

        [TestMethod]
        public void PrettyPrintNullOrEmpty_CompleteTest()
        {
            var expected = Strings.PrettyPrintNullOrEmpty((string)null);
            Assert.AreEqual("null", expected);

            expected = Strings.PrettyPrintNullOrEmpty(string.Empty);
            Assert.AreEqual("null", expected);

            expected = Strings.PrettyPrintNullOrEmpty("Hello World");
            Assert.AreNotEqual("null", expected);
            Assert.AreEqual("Hello World", expected);

        }

        #endregion

        #endregion
    }
}
