﻿namespace Arktos.WinBert.UnitTests
{
    using System;
    using System.IO;
    using System.Reflection;
    using Arktos.WinBert.Differencing;
    using Arktos.WinBert.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [DeploymentItem(@"test-assembly-files\", @"test-assembly-files\")]
    public class SimpleAssemblyDifferenceEngineTest
    {
        #region Fields & Constants

        private static readonly string srcDir = @"test-assembly-files\";

        private static readonly string diffAssembly1Path = srcDir + @"DiffTestAssembly1.dll";

        private static readonly string diffAssembly2Path = srcDir + @"DiffTestAssembly2.dll";

        #endregion

        #region Test Methods

        [TestMethod]
        public void Diff_DifferentAssemblies_Difference()
        {
            BertAssemblyDifferenceEngine target = new BertAssemblyDifferenceEngine(null);
            Assembly oldObject = this.LoadAssembly(diffAssembly1Path);
            Assembly newObject = this.LoadAssembly(diffAssembly2Path);

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
        public void Diff_IgnoreTargetsWithType_NoDiff()
        {
            IgnoreTarget[] targets = new IgnoreTarget[1];
            IgnoreTarget target0 = new IgnoreTarget(IgnoreType.Type, "BankAccount.BankAccount");
            targets[0] = target0;

            BertAssemblyDifferenceEngine target = new BertAssemblyDifferenceEngine(targets);
            Assembly oldObject = this.LoadAssembly(diffAssembly1Path);
            Assembly newObject = this.LoadAssembly(diffAssembly2Path);

            AssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.DifferenceResult);
            Assert.IsTrue(actual.TypeDifferences.Count <= 0);
        }

        [TestMethod]
        public void Diff_IgnoreTargetsWithMethod_NoDiff()
        {
            // create a mixed list of ignore targets
            IgnoreTarget[] targets = new IgnoreTarget[2];

            IgnoreTarget target0 = new IgnoreTarget(IgnoreType.Type, "Nonexistant.Type");
            targets[0] = target0;

            IgnoreTarget target1 = new IgnoreTarget(IgnoreType.Method, "Deposit");
            targets[1] = target1;

            BertAssemblyDifferenceEngine target = new BertAssemblyDifferenceEngine(targets);
            Assembly oldObject = this.LoadAssembly(diffAssembly1Path);
            Assembly newObject = this.LoadAssembly(diffAssembly2Path);

            AssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.DifferenceResult);
        }

        [TestMethod]
        public void Diff_IgnoreUnchangedMethodAndNonExistingType_Different()
        {
            // create a mixed list of ignore targets
            IgnoreTarget[] targets = new IgnoreTarget[2];

            IgnoreTarget target0 = new IgnoreTarget(IgnoreType.Type, "Nonexistant.Type");
            targets[0] = target0;

            IgnoreTarget target1 = new IgnoreTarget(IgnoreType.Method, "Withdraw");
            targets[1] = target1;

            BertAssemblyDifferenceEngine target = new BertAssemblyDifferenceEngine(targets);
            Assembly oldObject = this.LoadAssembly(diffAssembly1Path);
            Assembly newObject = this.LoadAssembly(diffAssembly2Path);

            AssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.DifferenceResult);
        }

        [TestMethod]
        public void Diff_IgnoreAllNamedMethods_NoDiff()
        {
            // create a mixed list of ignore targets
            IgnoreTarget[] targets = new IgnoreTarget[2];

            IgnoreTarget target0 = new IgnoreTarget(IgnoreType.Method, "Deposit");
            targets[0] = target0;

            IgnoreTarget target1 = new IgnoreTarget(IgnoreType.Method, "Withdraw");
            targets[1] = target1;

            BertAssemblyDifferenceEngine target = new BertAssemblyDifferenceEngine(targets);
            Assembly oldObject = this.LoadAssembly(diffAssembly1Path);
            Assembly newObject = this.LoadAssembly(diffAssembly2Path);

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
