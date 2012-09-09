namespace Arktos.WinBert.UnitTests
{
    using System;
    using System.IO;
    using Arktos.WinBert.Util;
    using Arktos.WinBert.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BuildVersionManagerTest
    {
        #region Fields and Constants
        
        private const string archivePath = @"test-archive";

        private const string testAssemblyV1Name = @"VersionManagerTestBankAccount1.dll";

        private const string testAssemblyV2Name = @"VersionManagerTestBankAccount2.dll";

        private BuildVersionManager versionManagerUnderTest;

        #endregion

        #region Test Plumbing
        
        [TestInitialize]
        public void TestInit()
        {
            // get a new build manager after each test
            this.versionManagerUnderTest = new BuildVersionManager(archivePath);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // delete everything inside the ARCHIVE_PATH directory.
            if (Directory.Exists(archivePath))
            {
                Directory.Delete(archivePath, true);
            }
        }

        #endregion

        #region Test Methods

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullArchivePath()
        {
            this.versionManagerUnderTest = new BuildVersionManager(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestLoadBuildNotExisting()
        {
            string pathToBuild = Path.Combine(archivePath, "Foo", testAssemblyV1Name);
            this.versionManagerUnderTest.LoadBuild(0, pathToBuild);
        }

        [TestMethod]                
        public void TestLoadBuildExisting()
        {
            string pathToBuild = Path.Combine(archivePath, "Foo", testAssemblyV1Name);

            Directory.CreateDirectory(Path.GetDirectoryName(pathToBuild));
            File.Copy(testAssemblyV1Name, pathToBuild);

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

        [TestMethod]
        public void TestNamedBuildManager()
        {
            string name = "foo";
            this.versionManagerUnderTest = new BuildVersionManager(archivePath, name);
            string expectedPath = Path.Combine(archivePath, name, this.versionManagerUnderTest.SequenceNumber.ToString(), testAssemblyV1Name);
            this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV1Name);

            Assert.AreEqual(this.versionManagerUnderTest.Name, name);

            Build mostRecent = this.versionManagerUnderTest.GetMostRecentBuild();

            Assert.IsNotNull(mostRecent);
            Assert.AreEqual(mostRecent.Path, expectedPath);
            Assert.IsTrue(File.Exists(expectedPath));
        }

        [TestMethod]
        public void TestAddBuild()
        {
            Build actualBuild = null;
            string expectedPath = null;
            
            expectedPath = Path.Combine(archivePath, this.versionManagerUnderTest.SequenceNumber.ToString(), testAssemblyV1Name);
            actualBuild = this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV1Name);

            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.Count == 1);
            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.ContainsValue(actualBuild));
            Assert.AreEqual(actualBuild.Path, expectedPath);

            expectedPath = Path.Combine(archivePath, this.versionManagerUnderTest.SequenceNumber.ToString(), testAssemblyV2Name);
            actualBuild = this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV2Name);

            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.Count == 2);
            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.ContainsValue(actualBuild));
            Assert.AreEqual(actualBuild.Path, expectedPath);
        }

        [TestMethod]
        public void TestAddBuildsWithSameName()
        {
            Build assembly1 = this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV1Name);
            Build assembly2 = this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV1Name);

            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.Count == 2);
            Assert.IsTrue(File.Exists(assembly1.Path));
            Assert.IsTrue(File.Exists(assembly2.Path));
        }

        [TestMethod]
        public void TestGetMostRecentBuildPath()
        {
            // save some space :D
            Build build = null;
            string expectedPath = null;

            // add a build and test to see if we get it back.
            expectedPath = Path.Combine(archivePath, this.versionManagerUnderTest.SequenceNumber.ToString(), testAssemblyV1Name);
            this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV1Name);
            build = this.versionManagerUnderTest.GetMostRecentBuild();
            Assert.IsTrue(File.Exists(build.Path));
            Assert.IsTrue(File.Exists(expectedPath));
            Assert.AreEqual(build.Path, expectedPath);

            // Add another build and test to see if we get it back
            expectedPath = Path.Combine(archivePath, this.versionManagerUnderTest.SequenceNumber.ToString(), testAssemblyV2Name);
            this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV2Name);
            build = this.versionManagerUnderTest.GetMostRecentBuild();
            Assert.IsTrue(File.Exists(build.Path));
            Assert.IsTrue(File.Exists(expectedPath));
            Assert.AreEqual(build.Path, expectedPath);
        }

        [TestMethod]
        public void TestBuildManagerRollover()
        {
            // first, make sure max archive size is correct
            Assert.AreEqual<byte>(byte.MaxValue, this.versionManagerUnderTest.MaxArchiveSize);

            // set the archive size to one
            this.versionManagerUnderTest.MaxArchiveSize = 1;

            // attempt to add two builds
            Build expected1 = this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV1Name);
            Build expected2 = this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV2Name);

            Assert.IsFalse(this.versionManagerUnderTest.BuildArchive.ContainsValue(expected1));
            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.ContainsValue(expected2));
            Assert.AreEqual(1, this.versionManagerUnderTest.BuildArchive.Count);
        }

        [TestMethod]
        public void TestBuildManagerTrim()
        {
            Build expected1 = this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV1Name);
            Build expected2 = this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV2Name);

            Assert.AreEqual<int>(2, this.versionManagerUnderTest.BuildArchive.Count);

            this.versionManagerUnderTest.MaxArchiveSize = 1;

            Assert.IsFalse(this.versionManagerUnderTest.BuildArchive.ContainsValue(expected1));
            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.ContainsValue(expected2));
            Assert.AreEqual(1, this.versionManagerUnderTest.BuildArchive.Count);
        }

        #endregion
    }
}