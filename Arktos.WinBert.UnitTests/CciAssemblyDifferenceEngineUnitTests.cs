namespace Arktos.WinBert.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Arktos.WinBert.Differencing.Cci;
    using Microsoft.Cci;
    using Arktos.WinBert.Xml;
    using System;

    [TestClass]
    [DeploymentItem(@"test-assembly-files\", @"test-assembly-files\")]
    public class CciAssemblyDifferenceEngineUnitTests
    {
        #region Fields & Constants

        private static readonly string srcDir = @"test-assembly-files\";

        private static readonly string diffAssembly1Path = srcDir + @"DiffTestAssembly1.dll";

        private static readonly string diffAssembly2Path = srcDir + @"DiffTestAssembly2.dll";

        private readonly IMetadataHost peHost;

        #endregion

        #region Test Methods

        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullArgument()
        {
            var target = new CciAssemblyDifferenceEngine(null);
        }

        #endregion

        #region Diff

        [TestMethod]
        public void Diff_DifferentAssemblies_Difference()
        {
            CciAssemblyDifferenceEngine target = new CciAssemblyDifferenceEngine();
            IAssembly oldObject = this.LoadAssembly(diffAssembly1Path);
            IAssembly newObject = this.LoadAssembly(diffAssembly2Path);

            ICciAssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.OldObject);
            Assert.IsNotNull(actual.NewObject);
            Assert.IsTrue(actual.IsDifferent);
            Assert.IsTrue(actual.TypeDifferences.Count > 0);

            foreach (var typeDiff in actual.TypeDifferences)
            {
                Assert.IsNotNull(typeDiff.OldObject);
                Assert.IsNotNull(typeDiff.NewObject);
            }
        }

        [TestMethod]
        public void Diff_IgnoreTargetsWithType_NoDiff()
        {
            IgnoreTarget[] targets = new IgnoreTarget[1];
            IgnoreTarget target0 = new IgnoreTarget(IgnoreType.Type, "BankAccount.BankAccount");
            targets[0] = target0;

            CciAssemblyDifferenceEngine target = new CciAssemblyDifferenceEngine(targets);
            IAssembly oldObject = this.LoadAssembly(diffAssembly1Path);
            IAssembly newObject = this.LoadAssembly(diffAssembly2Path);

            ICciAssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
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

            CciAssemblyDifferenceEngine target = new CciAssemblyDifferenceEngine(targets);
            IAssembly oldObject = this.LoadAssembly(diffAssembly1Path);
            IAssembly newObject = this.LoadAssembly(diffAssembly2Path);

            ICciAssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
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

            CciAssemblyDifferenceEngine target = new CciAssemblyDifferenceEngine(targets);
            IAssembly oldObject = this.LoadAssembly(diffAssembly1Path);
            IAssembly newObject = this.LoadAssembly(diffAssembly2Path);

            ICciAssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
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

            CciAssemblyDifferenceEngine target = new CciAssemblyDifferenceEngine(targets);
            IAssembly oldObject = this.LoadAssembly(diffAssembly1Path);
            IAssembly newObject = this.LoadAssembly(diffAssembly2Path);

            ICciAssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsDifferent);
        }

        #endregion

        #endregion

        #region Private Methods

        private IAssembly LoadAssembly(string path)
        {
            // BMK Implement me.
            return null;
        }

        #endregion
    }
}
