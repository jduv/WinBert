namespace Arktos.WinBert.UnitTests
{
    using System;
    using System.Reflection;
    using Arktos.WinBert.Differencing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AssemblyDifferenceResultUnitTests
    {
        #region Test Methods

        #region Constructors

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullArgument1()
        {
            var target = new AssemblyDifferenceResult(null, Assembly.GetExecutingAssembly());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullArgument2()
        {
            var target = new AssemblyDifferenceResult(Assembly.GetExecutingAssembly(), null);
        }

        #endregion

        #endregion
    }
}
