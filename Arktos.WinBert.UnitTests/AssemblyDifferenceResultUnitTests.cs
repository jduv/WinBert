namespace Arktos.WinBert.UnitTests
{
    using Arktos.WinBert.Differencing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Reflection;

    [TestClass]
    public class AssemblyDifferenceResultUnitTests
    {
        #region Test Methods

        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullOldAssembly()
        {
            var target = new AssemblyDifference(null, Assembly.GetExecutingAssembly(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullNewAssembly()
        {
            var target = new AssemblyDifference(Assembly.GetExecutingAssembly(), null, null);
        }

        [TestMethod]
        public void Ctor_NullTypeList()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var target = new AssemblyDifference(assembly, assembly, null);
            Assert.IsNotNull(target.TypeDifferences);
            Assert.IsFalse(target.AreDifferences);
            Assert.AreEqual(assembly.FullName, target.NewAssemblyTarget.FullName);
            Assert.AreSame(assembly.FullName, target.OldAssemblyTarget.FullName);
        }

        #endregion

        #endregion
    }
}
