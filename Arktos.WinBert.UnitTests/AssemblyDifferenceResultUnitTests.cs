﻿namespace Arktos.WinBert.UnitTests
{
    using System;
    using System.Reflection;
    using Arktos.WinBert.Differencing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AssemblyDifferenceResultUnitTests
    {
        #region Test Methods

        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullOldAssembly()
        {
            var target = new AssemblyDifferenceResult(null, Assembly.GetExecutingAssembly(), 0, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullNewAssembly()
        {
            var target = new AssemblyDifferenceResult(Assembly.GetExecutingAssembly(), null, 0, null);
        }

        [TestMethod]
        public void Ctor_NullTypeList()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var target = new AssemblyDifferenceResult(assembly, assembly, 0, null);
            Assert.AreEqual(0, target.ItemsCompared);
            Assert.IsNotNull(target.TypeDifferences);
            Assert.IsFalse(target.AreDifferences);
            Assert.AreEqual(assembly.FullName, target.NewAssemblyTarget.FullName);
            Assert.AreSame(assembly.FullName, target.OldAssemblyTarget.FullName);
        }

        #endregion

        #endregion
    }
}
