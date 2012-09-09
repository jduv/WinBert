namespace Arktos.WinBert.UnitTests
{
    using System;
    using System.IO;
    using Arktos.WinBert.Util;
    using Arktos.WinBert.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BuildVersionManagerTests
    {
        #region Fields and Constants
        
        private const string ArchivePath = @"test-archive\";

        private const string TestAssemblyDir = @"test-assembly-files\";

        private const string TestAssemblyV1Path = TestAssemblyDir + @"VersionManagerTestBankAccount1.dll";

        private const string TestAssemblyV2Path = TestAssemblyDir + @"VersionManagerTestBankAccount2.dll";

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

        #region Constructors
        
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
        [DeploymentItem(TestAssemblyV1Path, TestAssemblyDir)]
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
        public void Ctor_WithNameAndMaxArchiveSize_PropertiesSet()
        {
            byte maxBuilds = 100;
            string name = "MyBuildManager";

            var target = new BuildVersionManager(
                ArchivePath,
                maxBuilds,
                name);

            Assert.AreEqual(Path.GetFullPath(ArchivePath), target.ArchivePath, true);
            Assert.AreEqual(maxBuilds, target.MaxArchiveSize);
            Assert.AreEqual(name, target.Name, true);
        }

        #endregion

        ////[TestMethod]
        ////[ExpectedException(typeof(ArgumentException))]
        ////public void TestLoadBuildNotExisting()
        ////{
        ////    string pathToBuild = Path.Combine(ArchivePath, "Foo", TestAssemblyV1Path);
        ////    this.versionManagerUnderTest.LoadBuild(0, pathToBuild);
        ////}

        ////[TestMethod]                
        ////public void TestLoadBuildExisting()
        ////{
        ////    string pathToBuild = Path.Combine(ArchivePath, "Foo", TestAssemblyV1Path);

        ////    Directory.CreateDirectory(Path.GetDirectoryName(pathToBuild));
        ////    File.Copy(TestAssemblyV1Path, pathToBuild);

        ////    if (File.Exists(pathToBuild))
        ////    {
        ////        this.versionManagerUnderTest.LoadBuild(0, pathToBuild);
        ////    }
        ////    else
        ////    {
        ////        string error = String.Format("Could not copy the test file to the proper directory! Path: {0}", pathToBuild);
        ////        Assert.Fail(error);
        ////    }
        ////}

        ////[TestMethod]
        ////public void TestNamedBuildManager()
        ////{
        ////    string name = "foo";
        ////    this.versionManagerUnderTest = new BuildVersionManager(ArchivePath, name);
        ////    string expectedPath = Path.Combine(ArchivePath, name, this.versionManagerUnderTest.SequenceNumber.ToString(), TestAssemblyV1Path);
        ////    this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);

        ////    Assert.AreEqual(this.versionManagerUnderTest.Name, name);

        ////    Build mostRecent = this.versionManagerUnderTest.GetMostRecentBuild();

        ////    Assert.IsNotNull(mostRecent);
        ////    Assert.AreEqual(mostRecent.Path, expectedPath);
        ////    Assert.IsTrue(File.Exists(expectedPath));
        ////}

        ////[TestMethod]
        ////public void TestAddBuild()
        ////{
        ////    Build actualBuild = null;
        ////    string expectedPath = null;
            
        ////    expectedPath = Path.Combine(ArchivePath, this.versionManagerUnderTest.SequenceNumber.ToString(), TestAssemblyV1Path);
        ////    actualBuild = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);

        ////    Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.Count == 1);
        ////    Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.ContainsValue(actualBuild));
        ////    Assert.AreEqual(actualBuild.Path, expectedPath);

        ////    expectedPath = Path.Combine(ArchivePath, this.versionManagerUnderTest.SequenceNumber.ToString(), TestAssemblyV2Path);
        ////    actualBuild = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV2Path);

        ////    Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.Count == 2);
        ////    Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.ContainsValue(actualBuild));
        ////    Assert.AreEqual(actualBuild.Path, expectedPath);
        ////}

        ////[TestMethod]
        ////public void TestAddBuildsWithSameName()
        ////{
        ////    Build assembly1 = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);
        ////    Build assembly2 = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);

        ////    Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.Count == 2);
        ////    Assert.IsTrue(File.Exists(assembly1.Path));
        ////    Assert.IsTrue(File.Exists(assembly2.Path));
        ////}

        ////[TestMethod]
        ////public void TestGetMostRecentBuildPath()
        ////{
        ////    // save some space :D
        ////    Build build = null;
        ////    string expectedPath = null;

        ////    // add a build and test to see if we get it back.
        ////    expectedPath = Path.Combine(ArchivePath, this.versionManagerUnderTest.SequenceNumber.ToString(), TestAssemblyV1Path);
        ////    this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);
        ////    build = this.versionManagerUnderTest.GetMostRecentBuild();
        ////    Assert.IsTrue(File.Exists(build.Path));
        ////    Assert.IsTrue(File.Exists(expectedPath));
        ////    Assert.AreEqual(build.Path, expectedPath);

        ////    // Add another build and test to see if we get it back
        ////    expectedPath = Path.Combine(ArchivePath, this.versionManagerUnderTest.SequenceNumber.ToString(), TestAssemblyV2Path);
        ////    this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV2Path);
        ////    build = this.versionManagerUnderTest.GetMostRecentBuild();
        ////    Assert.IsTrue(File.Exists(build.Path));
        ////    Assert.IsTrue(File.Exists(expectedPath));
        ////    Assert.AreEqual(build.Path, expectedPath);
        ////}

        ////[TestMethod]
        ////public void TestBuildManagerRollover()
        ////{
        ////    // first, make sure max archive size is correct
        ////    Assert.AreEqual<byte>(byte.MaxValue, this.versionManagerUnderTest.MaxArchiveSize);

        ////    // set the archive size to one
        ////    this.versionManagerUnderTest.MaxArchiveSize = 1;

        ////    // attempt to add two builds
        ////    Build expected1 = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);
        ////    Build expected2 = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV2Path);

        ////    Assert.IsFalse(this.versionManagerUnderTest.BuildArchive.ContainsValue(expected1));
        ////    Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.ContainsValue(expected2));
        ////    Assert.AreEqual(1, this.versionManagerUnderTest.BuildArchive.Count);
        ////}

        ////[TestMethod]
        ////public void TestBuildManagerTrim()
        ////{
        ////    Build expected1 = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV1Path);
        ////    Build expected2 = this.versionManagerUnderTest.AddNewSuccessfulBuild(TestAssemblyV2Path);

        ////    Assert.AreEqual<int>(2, this.versionManagerUnderTest.BuildArchive.Count);

        ////    this.versionManagerUnderTest.MaxArchiveSize = 1;

        ////    Assert.IsFalse(this.versionManagerUnderTest.BuildArchive.ContainsValue(expected1));
        ////    Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.ContainsValue(expected2));
        ////    Assert.AreEqual(1, this.versionManagerUnderTest.BuildArchive.Count);
        ////}

        #endregion
    }
}