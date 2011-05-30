namespace WinBertUnitTests
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WinBert.Differencing;
    using WinBert.Xml;

    /// <summary>
    /// This is a test class for SimpleAssemblyDifferenceEngineTest and is intended
    ///  to contain all SimpleAssemblyDifferenceEngineTest Unit Tests
    /// </summary>
    [TestClass]
    public class SimpleAssemblyDifferenceEngineTest
    {
        /// <summary>
        /// A test for Diff
        /// </summary>
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

        /// <summary>
        /// Test to see if ignore targets parameters work for ignoring types.
        /// </summary>
        [TestMethod]
        public void IgnoreTargetsTypeTest()
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

        /// <summary>
        /// Test to see if ignore targets parameters work for ignoring methods of the specified name.
        /// </summary>
        [TestMethod]
        public void IgnoreTargetsMethodTest1()
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

        /// <summary>
        /// Test to see if ignore targets parameters work for ignoring methods of the specified name.
        /// </summary>
        [TestMethod]
        public void IgnoreTargetsMethodTest2()
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

        /// <summary>
        /// Test to see if ignore targets parameters work for ignoring methods of the specified name.
        /// </summary>
        [TestMethod]
        public void IgnoreTargetsMethodTest3()
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

        /// <summary>
        /// This small utility method will load an assembly from the given path. In the event an error occurs, it will
        ///   Assert.Fail()
        /// </summary>
        /// <param name="path">
        /// The path of the assembly to load
        /// </param>
        /// <returns>
        /// A loaded assembly object of null on error
        /// </returns>
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
    }
}
