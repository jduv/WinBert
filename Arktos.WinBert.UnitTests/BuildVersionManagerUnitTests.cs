namespace Arktos.WinBert.UnitTests
{
    using System;
    using System.Linq;
    using System.IO;
    using Arktos.WinBert.Util;
    using Arktos.WinBert.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [DeploymentItem(@"test-assembly-files\", @"test-assembly-files\")]
    public class BuildVersionManagerUnitTests
    {
        #region Fields & Constants
        
        private static readonly string ArchivePath = @"test-archive\";

        private static readonly string TestAssemblyDir = @"test-assembly-files\";

        private static readonly string TestAssemblyV1Name = @"VersionManagerTestBankAccount1.dll";

        private static readonly string TestAssemblyV1Path = TestAssemblyDir + TestAssemblyV1Name;

        private static readonly string TestAssemblyV2Name = @"VersionManagerTestBankAccount2.dll";

        private static readonly string TestAssemblyV2Path = TestAssemblyDir + TestAssemblyV2Name;

        private BuildVersionManager versionManagerUnderTest;

        #endregion

        #region Test Plumbing
        
        [TestInitialize]
        public void TestInit()
        {
            // get a new build manager after each test
            this.versionManagerUnderTest = new BuildVersionManager(ArchivePath);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // delete everything inside the archive directory.
            if (Directory.Exists(ArchivePath))
            {
                Directory.Delete(ArchivePath, true);
            }
        }

        #endregion

        #region Test Methods

        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_NullArchivePath_ThrowsException()
        {
            var target = new BuildVersionManager(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_EmptyArchivePath_ThrowsException()
        {
            var target = new BuildVersionManager(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]        
        public void Ctor_ExistingFilePath_ThrowsException()
        {
            var target = new BuildVersionManager(TestAssemblyV1Path);
        }

        [TestMethod]
        public void Ctor_NonExistingDirectory_DirectoryCreated()
        {
            var path = Path.Combine(ArchivePath, "test-dir");
            var target = new BuildVersionManager(path);

            Assert.IsTrue(Directory.Exists(path));
            Assert.AreEqual(Path.GetFullPath(path), target.ArchivePath, true);
        }

        [TestMethod]
        public void Ctor_ValidPath_AssertDefaults()
        {
            var target = new BuildVersionManager(ArchivePath);
            Assert.IsNull(target.Name);
            Assert.AreEqual(byte.MaxValue, target.MaxArchiveSize);
            Assert.AreEqual(Path.GetFullPath(ArchivePath), target.ArchivePath, true);
        }

        [TestMethod]
        public void Ctor_WithNameAndMaxArchiveSize_PropertiesSet()
        {
            byte maxBuilds = 100;
            string name = "MyBuildManager";

            var target = new BuildVersionManager(
                ArchivePath,
                maxBuilds,
                name);

            var expectedPath = Path.Combine(Path.GetFullPath(ArchivePath), name);
            Assert.AreEqual(expectedPath, target.ArchivePath, true);
            Assert.AreEqual(maxBuilds, target.MaxArchiveSize);
            Assert.AreEqual(name, target.Name, true);
        }

        [TestMethod]
        public void Ctor_NoNameAndMaxArchiveSize_PropertiesSet()
        {
            byte maxBuilds = 100;
            var target = new BuildVersionManager(ArchivePath, maxBuilds);

            Assert.AreEqual(Path.GetFullPath(ArchivePath), target.ArchivePath, true);
            Assert.AreEqual(maxBuilds, target.MaxArchiveSize);
            Assert.IsNull(target.Name);
        }

        [TestMethod]
        public void Ctor_ValidName_CorrectPathsWhenLoadingRecentBuild()
        {
            string name = "foo";
            this.versionManagerUnderTest = new BuildVersionManager(ArchivePath, name: name);
            
            string expectedPath = Path.Combine(
                ArchivePath, 
                name, 
                this.versionManagerUnderTest.SequenceNumber.ToString(), 
                TestAssemblyV1Name);
            
            this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);
            Build mostRecent = this.versionManagerUnderTest.GetMostRecentBuild();

            Assert.AreEqual(this.versionManagerUnderTest.Name, name);
            Assert.IsNotNull(mostRecent);
            Assert.AreEqual(mostRecent.AssemblyPath, Path.GetFullPath(expectedPath), true);
            Assert.IsTrue(File.Exists(expectedPath));
        }

        #endregion

        #region Properties

        [TestMethod]
        public void MaxArchiveSizeProperty_SetToLessThanBuildCount_TrimsArchive()
        {
            Build expected1 = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);
            Build expected2 = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV2Path);

            Assert.AreEqual(2, this.versionManagerUnderTest.BuildArchive.Count, "Test case not set up properly; unable to add a build.");

            this.versionManagerUnderTest.MaxArchiveSize = 1;

            Assert.IsFalse(this.versionManagerUnderTest.BuildArchive.ContainsValue(expected1));
            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.ContainsValue(expected2));
            Assert.AreEqual(1, this.versionManagerUnderTest.BuildArchive.Count);
        }

        [TestMethod]
        public void MaxArchiveSizeProperty_SetToZero_ClearsArchive()
        {
            Build expected1 = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);
            Build expected2 = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV2Path);

            Assert.AreEqual(2, this.versionManagerUnderTest.BuildArchive.Count, "Test case not set up properly; unable to add a build.");

            this.versionManagerUnderTest.MaxArchiveSize = 0;

            Assert.IsFalse(this.versionManagerUnderTest.BuildArchive.ContainsValue(expected1));
            Assert.IsFalse(this.versionManagerUnderTest.BuildArchive.ContainsValue(expected2));
            Assert.AreEqual(0, this.versionManagerUnderTest.BuildArchive.Count);
        }

        #endregion

        #region LoadBuild

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadBuild_MaxUintSequenceNumber()
        {
            var pathToBuild = Path.Combine(ArchivePath, "Foo", TestAssemblyV1Path);
            this.versionManagerUnderTest.LoadBuild(uint.MaxValue, pathToBuild);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadBuild_NonExistingBuild()
        {
            var pathToBuild = Path.Combine(ArchivePath, "Foo", TestAssemblyV1Path);
            this.versionManagerUnderTest.LoadBuild(0, pathToBuild);
        }

        [TestMethod]
        public void LoadBuild_ExistingBuild()
        {
            var pathToBuild = Path.Combine(ArchivePath, "Foo", TestAssemblyV1Path);

            Directory.CreateDirectory(Path.GetDirectoryName(pathToBuild));
            File.Copy(TestAssemblyV1Path, pathToBuild);

            if (File.Exists(pathToBuild))
            {
                this.versionManagerUnderTest.LoadBuild(0, pathToBuild);
            }
            else
            {
                string error = String.Format("Could not copy the test file to the proper directory! Path: {0}", pathToBuild);
                Assert.Fail(error);
            }
        }

        #endregion

        #region AddNewSuccessfulBuild

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddNewSuccessfulBuild_BadPath()
        {
            this.versionManagerUnderTest.AddNewSuccessfulBuild(Guid.NewGuid().ToString().Take(10) + ".dll");
        }

        [TestMethod]
        public void AddNewSuccessfulBuild_TwoBuilds()
        {
            Build actualBuild = null;
            string expectedPath = null;

            expectedPath = Path.Combine(
                ArchivePath, 
                this.versionManagerUnderTest.SequenceNumber.ToString(), 
                TestAssemblyV1Name);

            actualBuild = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);

            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.Count == 1);
            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.ContainsValue(actualBuild));
            Assert.AreEqual(actualBuild.AssemblyPath, Path.GetFullPath(expectedPath));

            expectedPath = Path.Combine(
                ArchivePath, 
                this.versionManagerUnderTest.SequenceNumber.ToString(), 
                TestAssemblyV2Name);

            actualBuild = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV2Path);

            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.Count == 2);
            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.ContainsValue(actualBuild));
            Assert.AreEqual(actualBuild.AssemblyPath, Path.GetFullPath(expectedPath));
        }

        [TestMethod]
        public void AddNewSuccessfulBuild_DuplicateNames()
        {
            Build assembly1 = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);
            Build assembly2 = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);

            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.Count == 2);
            Assert.IsTrue(File.Exists(assembly1.AssemblyPath));
            Assert.IsTrue(File.Exists(assembly2.AssemblyPath));
        }

        [TestMethod]
        public void AddNewSuccessfulBuild_LowMaxValue_Rollover()
        {
            // first, make sure max archive size is correct
            Assert.AreEqual<byte>(byte.MaxValue, this.versionManagerUnderTest.MaxArchiveSize);

            // set the archive size to one
            this.versionManagerUnderTest.MaxArchiveSize = 1;

            // attempt to add two builds
            Build expected1 = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);
            Build expected2 = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV2Path);

            Assert.IsFalse(this.versionManagerUnderTest.BuildArchive.ContainsValue(expected1));
            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.ContainsValue(expected2));
            Assert.AreEqual(1, this.versionManagerUnderTest.BuildArchive.Count);
        }

        #endregion

        #region GetMostRecentBuild

        [TestMethod]
        public void GetMostRecentBuild_EmptyManager_NullResult()
        {
            var build = this.versionManagerUnderTest.GetMostRecentBuild();
            Assert.IsNull(build);
        }

        [TestMethod]
        public void GetMostRecentBuild_MultipleBuildsAdded_IncrementsCorrectly()
        {
            // save some space :D
            Build build = null;
            string expectedPath = null;

            // add a build and test to see if we get it back.
            expectedPath = Path.Combine(
                ArchivePath, 
                this.versionManagerUnderTest.SequenceNumber.ToString(),
                TestAssemblyV1Name);

            this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);
            build = this.versionManagerUnderTest.GetMostRecentBuild();
            Assert.IsTrue(File.Exists(build.AssemblyPath));
            Assert.IsTrue(File.Exists(expectedPath));
            Assert.AreEqual(build.AssemblyPath, Path.GetFullPath(expectedPath));

            // Add another build and test to see if we get it back
            expectedPath = Path.Combine(
                ArchivePath, 
                this.versionManagerUnderTest.SequenceNumber.ToString(), 
                TestAssemblyV2Name);

            this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV2Path);
            build = this.versionManagerUnderTest.GetMostRecentBuild();
            Assert.IsTrue(File.Exists(build.AssemblyPath));
            Assert.IsTrue(File.Exists(expectedPath));
            Assert.AreEqual(build.AssemblyPath, Path.GetFullPath(expectedPath));
        }

        #endregion

        #region GetBuildRevision

        [TestMethod]        
        public void GetBuildRevision_EmptyManager_NullResult()
        {
            var build = this.versionManagerUnderTest.GetBuildRevision(0);
            Assert.IsNull(build);
        }

        [TestMethod]
        public void GetBuildRevision_OneBuildLoaded_ValidBuildReturnedAtIndex()
        {
            var expectedPath = Path.Combine(
                ArchivePath,
                this.versionManagerUnderTest.SequenceNumber.ToString(),
                TestAssemblyV1Name);

            var expectedBuild = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);
            Assert.IsNotNull(expectedBuild, "Test case not set up properly; unable to create expected build.");

            var actualBuild = this.versionManagerUnderTest.GetBuildRevision(0);
            Assert.IsNotNull(actualBuild);
            Assert.AreEqual(expectedBuild, actualBuild);
        }

        #endregion

        #region GetBuildRevisionPreceding

        [TestMethod]
        public void GetBuildRevisionPreceding_EmptyManager_NullResult()
        {
            var build = this.versionManagerUnderTest.GetBuildRevisionPreceding(0);
            Assert.IsNull(build);
        }

        [TestMethod]
        public void GetBuildRevisionPreceding_OneBuildLoaded_NullResult()
        {
            var expectedPath = Path.Combine(
                ArchivePath,
                this.versionManagerUnderTest.SequenceNumber.ToString(),
                TestAssemblyV1Name);

            var expectedBuild = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);
            Assert.IsNotNull(expectedBuild, "Test case not set up properly; unable to create expected build.");

            var nullBuild = this.versionManagerUnderTest.GetBuildRevisionPreceding(0);
            Assert.IsNull(nullBuild);

            var precedingBuild = this.versionManagerUnderTest.GetBuildRevisionPreceding(expectedBuild.SequenceNumber);
            Assert.IsNull(precedingBuild);
        }

        [TestMethod]
        public void GetBuildRevisionPreceding_MultipleBuildsLoaded()
        {
            var path1 = Path.Combine(
                ArchivePath,
                this.versionManagerUnderTest.SequenceNumber.ToString(),
                TestAssemblyV1Name);

            var path2 = Path.Combine(
                ArchivePath,
                this.versionManagerUnderTest.SequenceNumber.ToString(),
                TestAssemblyV2Name);
            
            var expectedBuild = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);
            var precedingBuild = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV2Path);
            Assert.IsNotNull(expectedBuild, "Test case not set up properly; unable to create expected build.");
            Assert.IsNotNull(precedingBuild, "Test case not set up properly; unable to create preceding build.");

            var target = this.versionManagerUnderTest.GetBuildRevisionPreceding(precedingBuild.SequenceNumber);
            Assert.AreEqual(expectedBuild, target);
        }

        [TestMethod]
        public void GetBuildRevisionPreceding_BuildOverload()
        {
            var path1 = Path.Combine(
               ArchivePath,
               this.versionManagerUnderTest.SequenceNumber.ToString(),
               TestAssemblyV1Name);

            var path2 = Path.Combine(
                ArchivePath,
                this.versionManagerUnderTest.SequenceNumber.ToString(),
                TestAssemblyV2Name);

            var expectedBuild = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);
            var precedingBuild = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV2Path);
            Assert.IsNotNull(expectedBuild, "Test case not set up properly; unable to create expected build.");
            Assert.IsNotNull(precedingBuild, "Test case not set up properly; unable to create preceding build.");

            var build = this.versionManagerUnderTest.GetMostRecentBuild();
            var target = this.versionManagerUnderTest.GetBuildRevisionPreceding(build);

            Assert.AreEqual(expectedBuild, target);
        }

        #endregion

        #endregion
    }
}