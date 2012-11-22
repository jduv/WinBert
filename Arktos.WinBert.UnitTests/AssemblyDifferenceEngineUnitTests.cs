namespace Arktos.WinBert.UnitTests
{
    using System;
    using System.IO;
    using System.Reflection;
    using Arktos.WinBert.Differencing;
    using Arktos.WinBert.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [DeploymentItem(@"test-assembly-files\", @"test-assembly-files\")]
    public class AssemblyDifferenceEngineUnitTests
    {
        #region Fields & Constants

        private static readonly string srcDir = @"test-assembly-files\";

        private static readonly string diffAssembly1Path = srcDir + @"DiffTestAssembly1.dll";

        private static readonly string diffAssembly2Path = srcDir + @"DiffTestAssembly2.dll";

        #endregion

        #region Test Methods

        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullArgument()
        {
            var target = new AssemblyDifferenceEngine(null);
        }

        #endregion

        #region Diff

        [TestMethod]
        public void Diff_DifferentAssemblies_Difference()
        {
            AssemblyDifferenceEngine target = new AssemblyDifferenceEngine();
            Assembly oldObject = this.LoadAssembly(diffAssembly1Path);
            Assembly newObject = this.LoadAssembly(diffAssembly2Path);

            IAssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.OldAssemblyTarget);
            Assert.IsNotNull(actual.NewAssemblyTarget);
            Assert.IsTrue(actual.IsDifferent);
            Assert.IsTrue(actual.TypeDifferences.Count > 0);
        }

        [TestMethod]
        public void Diff_IgnoreTargetsWithType_NoDiff()
        {
            IgnoreTarget[] targets = new IgnoreTarget[1];
            IgnoreTarget target0 = new IgnoreTarget(IgnoreType.Type, "BankAccount.BankAccount");
            targets[0] = target0;

            AssemblyDifferenceEngine target = new AssemblyDifferenceEngine(targets);
            Assembly oldObject = this.LoadAssembly(diffAssembly1Path);
            Assembly newObject = this.LoadAssembly(diffAssembly2Path);

            IAssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.OldAssemblyTarget);
            Assert.IsNotNull(actual.NewAssemblyTarget);
            Assert.IsFalse(actual.IsDifferent);
            Assert.IsTrue(actual.TypeDifferences.Count == 0);
        }

        [TestMethod]
        public void Diff_IgnoreTargetsWithMethod_NoDiff()
        {
            // create a mixed list of ignore targets
            IgnoreTarget[] targets = new IgnoreTarget[2];

            IgnoreTarget target0 = new IgnoreTarget(IgnoreType.Type, "Nonexistant.Type");
            targets[0] = target0;

            IgnoreTarget target1 = new IgnoreTarget(IgnoreType.Method, "Withdraw");
            targets[1] = target1;

            AssemblyDifferenceEngine target = new AssemblyDifferenceEngine(targets);
            Assembly oldObject = this.LoadAssembly(diffAssembly1Path);
            Assembly newObject = this.LoadAssembly(diffAssembly2Path);

            IAssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.OldAssemblyTarget);
            Assert.IsNotNull(actual.NewAssemblyTarget);
            Assert.IsFalse(actual.IsDifferent);
        }

        [TestMethod]
        public void Diff_IgnoreUnchangedMethodAndNonExistingType_Different()
        {
            // create a mixed list of ignore targets
            IgnoreTarget[] targets = new IgnoreTarget[2];

            IgnoreTarget target0 = new IgnoreTarget(IgnoreType.Type, "Nonexistant.Type");
            targets[0] = target0;

            IgnoreTarget target1 = new IgnoreTarget(IgnoreType.Method, "Deposit");
            targets[1] = target1;

            AssemblyDifferenceEngine target = new AssemblyDifferenceEngine(targets);
            Assembly oldObject = this.LoadAssembly(diffAssembly1Path);
            Assembly newObject = this.LoadAssembly(diffAssembly2Path);

            IAssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.OldAssemblyTarget);
            Assert.IsNotNull(actual.NewAssemblyTarget);
            Assert.IsTrue(actual.IsDifferent);
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

            AssemblyDifferenceEngine target = new AssemblyDifferenceEngine(targets);
            Assembly oldObject = this.LoadAssembly(diffAssembly1Path);
            Assembly newObject = this.LoadAssembly(diffAssembly2Path);

            IAssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.OldAssemblyTarget);
            Assert.IsNotNull(actual.NewAssemblyTarget);
            Assert.IsFalse(actual.IsDifferent);
        }

        #endregion

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
