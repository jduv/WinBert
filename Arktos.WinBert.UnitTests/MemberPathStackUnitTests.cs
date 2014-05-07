namespace Arktos.WinBert.UnitTests
{
    using Arktos.WinBert.Differencing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class MemberPathStackUnitTests
    {
        #region Test Methods

        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_NullArgument()
        {
            var target = new MemberPathStack(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_EmptyArgument()
        {
            var target = new MemberPathStack(string.Empty);
        }

        #endregion

        #region CurrentPath Property

        [TestMethod]
        public void CurrentPathProp_ReturnsValidPath()
        {
            var expected = "roottype.path.to.field.or.property";
            var target = new MemberPathStack("RootType");
            target.Push("path");
            target.Push("to");
            target.Push("field");
            target.Push("or");
            target.Push("property");

            Assert.AreEqual(expected, target.CurrentPath);
        }

        #endregion

        #endregion
    }
}
