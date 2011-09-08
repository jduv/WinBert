namespace WinBertUnitTests
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WinBert.Util;
    using WinBert.Xml;

    /// <summary>
    /// Tests the BuildVersionManager class
    /// </summary>
    [TestClass]
    public class BuildVersionManagerTest
    {
        /// <summary>
        ///   Path to the new archive.
        /// </summary>
        private static readonly string archivePath = @"test-archive";

        /// <summary>
        ///   Name of a test assembly.
        /// </summary>
        private static readonly string testAssemblyV1Name = @"VersionManagerTestBankAccount1.dll";

        /// <summary>
        ///   Name of a test assembly
        /// </summary>
        private static readonly string testAssemblyV2Name = @"VersionManagerTestBankAccount2.dll";

        /// <summary>
        ///   The manager under test.
        /// </summary>
        private BuildVersionManager versionManagerUnderTest;

        /// <summary>
        /// Initializes properties before each test
        /// </summary>
        [TestInitialize]
        public void PreTestInit()
        {
            // get a new build manager after each test
            this.versionManagerUnderTest = new BuildVersionManager(archivePath);
        }

        /// <summary>
        /// Performs any cleanup after each test.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            // delete everything inside the ARCHIVE_PATH directory.
            if (Directory.Exists(archivePath))
            {
                Directory.Delete(archivePath, true);
            }
        }

        /// <summary>
        /// Test creating a build manager with a bad archive path (one that doesn't exist).
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullArchivePath()
        {
            this.versionManagerUnderTest = new BuildVersionManager(null);
        }

        /// <summary>
        /// Tests loading a non-existent build using the LoadBuild(uint, string) method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestLoadBuildNotExisting()
        {
            string pathToBuild = Path.Combine(archivePath, "Foo", testAssemblyV1Name);

            this.versionManagerUnderTest.LoadBuild(0, pathToBuild);
        }

        /// <summary>
        /// Tests loading an existing build using the LoadBuild(uint, string) method.
        /// </summary>
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

        /// <summary>
        /// Test creating a build manager with a name.
        /// </summary>
        [TestMethod]
        public void TestNamedBuildManager()
        {
            string name = "foo";
            this.versionManagerUnderTest = new BuildVersionManager(archivePath, name);
            string expectedPath = Path.Combine(archivePath, name, this.versionManagerUnderTest.SequenceNumber.ToString(), testAssemblyV1Name);
            this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV1Name);

            Assert.AreEqual<string>(this.versionManagerUnderTest.Name, name);

            Build mostRecent = this.versionManagerUnderTest.GetMostRecentBuild();

            Assert.IsNotNull(mostRecent);
            Assert.AreEqual<string>(mostRecent.Path, expectedPath);
            Assert.IsTrue(File.Exists(expectedPath));
        }

        /// <summary>
        /// Test adding a couple of build paths to the version manager and ensure that the list is
        ///   in it's proper state.
        /// </summary>
        [TestMethod]
        public void TestAddBuild()
        {
            Build actualBuild = null;
            string expectedPath = null;
            
            expectedPath = Path.Combine(archivePath, this.versionManagerUnderTest.SequenceNumber.ToString(), testAssemblyV1Name);
            actualBuild = this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV1Name);

            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.Count == 1);
            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.ContainsValue(actualBuild));
            Assert.AreEqual<string>(actualBuild.Path, expectedPath);

            expectedPath = Path.Combine(archivePath, this.versionManagerUnderTest.SequenceNumber.ToString(), testAssemblyV2Name);
            actualBuild = this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV2Name);

            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.Count == 2);
            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.ContainsValue(actualBuild));
            Assert.AreEqual<string>(actualBuild.Path, expectedPath);
        }

        /// <summary>
        /// Tests to see if adding an assembly with the same name yields a collision. It shouldn't as this is a very
        ///   common usage scenario.
        /// </summary>
        [TestMethod]
        public void TestAddBuildsWithSameName()
        {
            Build assembly1 = this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV1Name);
            Build assembly2 = this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV1Name);

            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.Count == 2);
            Assert.IsTrue(File.Exists(assembly1.Path));
            Assert.IsTrue(File.Exists(assembly2.Path));
        }

        /// <summary>
        /// Test getting the most recent build path from the version manager.
        /// </summary>
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
            Assert.AreEqual<string>(build.Path, expectedPath);

            // Add another build and test to see if we get it back
            expectedPath = Path.Combine(archivePath, this.versionManagerUnderTest.SequenceNumber.ToString(), testAssemblyV2Name);
            this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV2Name);
            build = this.versionManagerUnderTest.GetMostRecentBuild();
            Assert.IsTrue(File.Exists(build.Path));
            Assert.IsTrue(File.Exists(expectedPath));
            Assert.AreEqual<string>(build.Path, expectedPath);
        }

        /// <summary>
        /// Attempt to roll the build manager over by adding more than the maximum number of build paths to the manager.
        /// </summary>
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
            Assert.AreEqual<int>(1, this.versionManagerUnderTest.BuildArchive.Count);
        }

        /// <summary>
        /// Test the trim functionality of the build manager.
        /// </summary>
        [TestMethod]
        public void TestBuildManagerTrim()
        {
            Build expected1 = this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV1Name);
            Build expected2 = this.versionManagerUnderTest.AddNewSuccessfulBuild(testAssemblyV2Name);

            Assert.AreEqual<int>(2, this.versionManagerUnderTest.BuildArchive.Count);

            this.versionManagerUnderTest.MaxArchiveSize = 1;

            Assert.IsFalse(this.versionManagerUnderTest.BuildArchive.ContainsValue(expected1));
            Assert.IsTrue(this.versionManagerUnderTest.BuildArchive.ContainsValue(expected2));
            Assert.AreEqual<int>(1, this.versionManagerUnderTest.BuildArchive.Count);
        }
    }
}