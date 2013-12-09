namespace Arktos.WinBert.UnitTests
{
    using System;
    using System.Linq;
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

        private static readonly string interfacesOnly1Path = srcDir + @"InterfaceTestAssembly1.dll";

        private static readonly string interfacesOnly2Path = srcDir + @"InterfaceTestAssembly2.dll";

        #endregion

        #region Test Methods

        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullIgnoreTargets()
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
            Assert.IsTrue(actual.AreDifferences);
            Assert.IsTrue(actual.TypeDifferences.Count() > 0);
        }

        [TestMethod]
        public void Diff_DiffIgnoreTargetsWithType_NoDiff()
        {
            DiffIgnoreTarget[] targets = new DiffIgnoreTarget[1];
            DiffIgnoreTarget target0 = new DiffIgnoreTarget(DiffIgnoreType.Type, "BankAccount.BankAccount");
            targets[0] = target0;

            AssemblyDifferenceEngine target = new AssemblyDifferenceEngine(targets);
            Assembly oldObject = this.LoadAssembly(diffAssembly1Path);
            Assembly newObject = this.LoadAssembly(diffAssembly2Path);

            IAssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.OldAssemblyTarget);
            Assert.IsNotNull(actual.NewAssemblyTarget);
            Assert.IsFalse(actual.AreDifferences);
            Assert.IsTrue(actual.TypeDifferences.Count() == 0);
        }

        [TestMethod]
        public void Diff_DiffIgnoreTargetsWithMethod_NoDiff()
        {
            // create a mixed list of ignore targets
            DiffIgnoreTarget[] targets = new DiffIgnoreTarget[2];

            DiffIgnoreTarget target0 = new DiffIgnoreTarget(DiffIgnoreType.Type, "Nonexistant.Type");
            targets[0] = target0;

            DiffIgnoreTarget target1 = new DiffIgnoreTarget(DiffIgnoreType.Method, "Withdraw");
            targets[1] = target1;

            AssemblyDifferenceEngine target = new AssemblyDifferenceEngine(targets);
            Assembly oldObject = this.LoadAssembly(diffAssembly1Path);
            Assembly newObject = this.LoadAssembly(diffAssembly2Path);

            IAssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.OldAssemblyTarget);
            Assert.IsNotNull(actual.NewAssemblyTarget);
            Assert.IsFalse(actual.AreDifferences);
        }

        [TestMethod]
        public void Diff_IgnoreUnchangedMethodAndNonExistingType_Different()
        {
            // create a mixed list of ignore targets
            DiffIgnoreTarget[] targets = new DiffIgnoreTarget[2];

            DiffIgnoreTarget target0 = new DiffIgnoreTarget(DiffIgnoreType.Type, "Nonexistant.Type");
            targets[0] = target0;

            DiffIgnoreTarget target1 = new DiffIgnoreTarget(DiffIgnoreType.Method, "Deposit");
            targets[1] = target1;

            AssemblyDifferenceEngine target = new AssemblyDifferenceEngine(targets);
            Assembly oldObject = this.LoadAssembly(diffAssembly1Path);
            Assembly newObject = this.LoadAssembly(diffAssembly2Path);

            IAssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.OldAssemblyTarget);
            Assert.IsNotNull(actual.NewAssemblyTarget);
            Assert.IsTrue(actual.AreDifferences);
        }

        [TestMethod]
        public void Diff_IgnoreAllNamedMethods_NoDiff()
        {
            // create a mixed list of ignore targets
            DiffIgnoreTarget[] targets = new DiffIgnoreTarget[2];

            DiffIgnoreTarget target0 = new DiffIgnoreTarget(DiffIgnoreType.Method, "Deposit");
            targets[0] = target0;

            DiffIgnoreTarget target1 = new DiffIgnoreTarget(DiffIgnoreType.Method, "Withdraw");
            targets[1] = target1;

            AssemblyDifferenceEngine target = new AssemblyDifferenceEngine(targets);
            Assembly oldObject = this.LoadAssembly(diffAssembly1Path);
            Assembly newObject = this.LoadAssembly(diffAssembly2Path);

            IAssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.OldAssemblyTarget);
            Assert.IsNotNull(actual.NewAssemblyTarget);
            Assert.IsFalse(actual.AreDifferences);
        }

        [TestMethod]
        public void Diff_InterfacesOnly_Ignored()
        {
            AssemblyDifferenceEngine target = new AssemblyDifferenceEngine();
            Assembly oldObject = this.LoadAssembly(interfacesOnly1Path);
            Assembly newObject = this.LoadAssembly(interfacesOnly2Path);

            IAssemblyDifferenceResult actual = target.Diff(oldObject, newObject);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.OldAssemblyTarget);
            Assert.IsNotNull(actual.NewAssemblyTarget);
            Assert.IsFalse(actual.AreDifferences);
            Assert.AreEqual(0, actual.ItemsCompared);
            Assert.AreEqual(0, actual.TypeDifferences.Count());
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
