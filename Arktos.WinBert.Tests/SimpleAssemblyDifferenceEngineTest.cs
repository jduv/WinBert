namespace Arktos.WinBertUnitTests
{
    using System;
    using System.IO;
    using System.Reflection;
    using Arktos.WinBert.Differencing;
    using Arktos.WinBert.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SimpleAssemblyDifferenceEngineTest
    {
        #region Test Methods

        [TestMethod]
        public void DiffTest()
        {
            BertAssemblyDifferenceEngine target = new BertAssemblyDifferenceEngine(null); // TODO: Initialize to an appropriate value
            Assembly oldObject = this.LoadAssembly(@"DiffTestAssembly1.dll");
            Assembly newObject = this.LoadAssembly(@"DiffTestAssembly2.dll");

            AssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.OldObject);
            Assert.IsNotNull(actual.NewObject);
            Assert.IsInstanceOfType(actual.OldObject, typeof(Assembly));
            Assert.IsInstanceOfType(actual.NewObject, typeof(Assembly));
            Assert.IsTrue(actual.DifferenceResult);
            Assert.IsInstanceOfType(actual, typeof(AssemblyDifferenceResult));
        }

        [TestMethod]
        public void IgnoreTargetsIgnoreType()
        {
            IgnoreTarget[] targets = new IgnoreTarget[1];
            IgnoreTarget target0 = new IgnoreTarget(IgnoreType.Type, "BankAccount.BankAccount");
            targets[0] = target0;

            BertAssemblyDifferenceEngine target = new BertAssemblyDifferenceEngine(targets); // TODO: Initialize to an appropriate value
            Assembly oldObject = this.LoadAssembly(@"DiffTestAssembly1.dll");
            Assembly newObject = this.LoadAssembly(@"DiffTestAssembly2.dll");

            AssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.DifferenceResult);
            Assert.IsTrue(actual.TypeDifferences.Count <= 0);
        }

        [TestMethod]
        public void IgnoreTargetsIsMethodIgnored()
        {
            // create a mixed list of ignore targets
            IgnoreTarget[] targets = new IgnoreTarget[2];
            
            IgnoreTarget target0 = new IgnoreTarget(IgnoreType.Type, "Nonexistant.Type");
            targets[0] = target0;

            IgnoreTarget target1 = new IgnoreTarget(IgnoreType.Method, "Deposit");
            targets[1] = target1;

            BertAssemblyDifferenceEngine target = new BertAssemblyDifferenceEngine(targets);
            Assembly oldObject = this.LoadAssembly(@"DiffTestAssembly1.dll");
            Assembly newObject = this.LoadAssembly(@"DiffTestAssembly2.dll");

            AssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.DifferenceResult);
        }

        [TestMethod]
        public void IgnoreTargetsValidDifference()
        {
            // create a mixed list of ignore targets
            IgnoreTarget[] targets = new IgnoreTarget[2];

            IgnoreTarget target0 = new IgnoreTarget(IgnoreType.Type, "Nonexistant.Type");
            targets[0] = target0;

            IgnoreTarget target1 = new IgnoreTarget(IgnoreType.Method, "Withdraw");
            targets[1] = target1;

            BertAssemblyDifferenceEngine target = new BertAssemblyDifferenceEngine(targets);
            Assembly oldObject = this.LoadAssembly(@"DiffTestAssembly1.dll");
            Assembly newObject = this.LoadAssembly(@"DiffTestAssembly2.dll");

            AssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.DifferenceResult);
        }

        [TestMethod]
        public void IgnoreTargetsValidMethods()
        {
            // create a mixed list of ignore targets
            IgnoreTarget[] targets = new IgnoreTarget[2];

            IgnoreTarget target0 = new IgnoreTarget(IgnoreType.Method, "Deposit");
            targets[0] = target0;

            IgnoreTarget target1 = new IgnoreTarget(IgnoreType.Method, "Withdraw");
            targets[1] = target1;

            BertAssemblyDifferenceEngine target = new BertAssemblyDifferenceEngine(targets);
            Assembly oldObject = this.LoadAssembly(@"DiffTestAssembly1.dll");
            Assembly newObject = this.LoadAssembly(@"DiffTestAssembly2.dll");

            AssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.DifferenceResult);
        }

        #endregion

        #region Private Methods

        private Assembly LoadAssembly(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return Assembly.LoadFile(Path.GetFullPath(path));
                }
            }
            catch (Exception exception)
            {
                string errorMsg = string.Format("Error loading assembly with path {0}. {1}", path, exception.ToString());
                Assert.Fail(errorMsg);
            }

            return null;
        }

        #endregion
    }
}
