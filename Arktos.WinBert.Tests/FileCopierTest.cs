namespace Arktos.WinBertUnitTests
{
    using System;
    using System.IO;
    using Arktos.WinBert.Util;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FileCopierTest
    {
        #region Fields and Constants

        private static readonly string destDir = @"filecopier-test-files-deploy\";

        private static readonly string sourceFile = @"src.txt";

        private static readonly string existingDestTxt = "existingDest.txt";

        private static readonly string readOnlyDestTxt = "readOnlyDest.txt";

        private FileCopier fileCopierUnderTest = null;

        #endregion

        #region Test Plumbing

        [TestInitialize]
        public void TestInit()
        {
            // make sure the destination directory exists
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            File.Copy(existingDestTxt, Path.Combine(destDir, existingDestTxt), true);
            File.Copy(readOnlyDestTxt, Path.Combine(destDir, readOnlyDestTxt), true);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (Directory.Exists(destDir))
            {
                // set read only to false on the test file or we cannot delete it per deployment
                string readOnlyFile = Path.Combine(destDir, readOnlyDestTxt);

                if (File.Exists(readOnlyFile))
                {
                    FileInfo info = new FileInfo(readOnlyFile);
                    info.IsReadOnly = false;
                }

                Directory.Delete(destDir, true);
            }
        }

        #endregion

        #region Test Methods

        [TestMethod]
        public void TestWithNoFlags()
        {
            this.fileCopierUnderTest = new FileCopier();

            Assert.IsTrue(this.CopyToDirectory(this.fileCopierUnderTest));
            Assert.IsTrue(this.CopyFileToNamedTarget(this.fileCopierUnderTest));
            Assert.IsFalse(this.CopyToExistingFile(this.fileCopierUnderTest));
            Assert.IsFalse(this.CopyToReadOnlyFile(this.fileCopierUnderTest));
            Assert.IsFalse(this.CopyToNonExistantDirectory(this.fileCopierUnderTest));
        }

        [TestMethod]
        public void TestFlagBasicOverwritable()
        {
            this.fileCopierUnderTest = new FileCopier(FileCopierFlags.AlwaysOverwriteDestination);

            Assert.IsTrue(this.CopyToDirectory(this.fileCopierUnderTest));
            Assert.IsTrue(this.CopyFileToNamedTarget(this.fileCopierUnderTest));
            Assert.IsTrue(this.CopyToExistingFile(this.fileCopierUnderTest));
            Assert.IsFalse(this.CopyToReadOnlyFile(this.fileCopierUnderTest));
            Assert.IsFalse(this.CopyToNonExistantDirectory(this.fileCopierUnderTest));
        }

        [TestMethod]
        public void TestFlagReadOnlyOverwritable()
        {
            this.fileCopierUnderTest = new FileCopier(FileCopierFlags.AlwaysOverwriteDestination |
                                                      FileCopierFlags.AttemptOverwriteReadonlyDest);

            Assert.IsTrue(this.CopyToDirectory(this.fileCopierUnderTest));
            Assert.IsTrue(this.CopyFileToNamedTarget(this.fileCopierUnderTest));
            Assert.IsTrue(this.CopyToExistingFile(this.fileCopierUnderTest));
            Assert.IsTrue(this.CopyToReadOnlyFile(this.fileCopierUnderTest));
            Assert.IsFalse(this.CopyToNonExistantDirectory(this.fileCopierUnderTest));
        }

        [TestMethod]
        public void TestFlagCreateDirectory()
        {
            this.fileCopierUnderTest = new FileCopier(FileCopierFlags.CreateDestinationDirectorys);

            Assert.IsTrue(this.CopyToDirectory(this.fileCopierUnderTest));
            Assert.IsTrue(this.CopyFileToNamedTarget(this.fileCopierUnderTest));
            Assert.IsFalse(this.CopyToExistingFile(this.fileCopierUnderTest));
            Assert.IsFalse(this.CopyToReadOnlyFile(this.fileCopierUnderTest));
            Assert.IsTrue(this.CopyToNonExistantDirectory(this.fileCopierUnderTest));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCopySourceDirectory()
        {
            this.fileCopierUnderTest = new FileCopier();
            this.fileCopierUnderTest.TryCopyFile(destDir, destDir);
        }

        #endregion

        #region Private Methods

        private bool CopyToDirectory(FileCopier copier)
        {
            return copier.TryCopyFile(sourceFile, destDir);
        }

        private bool CopyFileToNamedTarget(FileCopier copier)
        {
            string destPath = Path.Combine(destDir, @"out.txt");
            return copier.TryCopyFile(sourceFile, destPath);
        }

        private bool CopyToExistingFile(FileCopier copier)
        {
            string destPath = Path.Combine(destDir, @"existingDest.txt");

            if (File.Exists(destPath))
            {
                return copier.TryCopyFile(sourceFile, destPath);
            }
            else
            {
                Assert.Fail("The data for this test case has not been properly deployed! Missing file: " + destPath);
                return false;
            }
        }

        private bool CopyToReadOnlyFile(FileCopier copier)
        {
            string destPath = Path.Combine(destDir, @"readOnlyDest.txt");

            if (File.Exists(destPath))
            {
                FileInfo info = new FileInfo(destPath);

                // ensure that the file is readonly.
                if (!info.IsReadOnly)
                {
                    info.IsReadOnly = true;
                }

                return copier.TryCopyFile(sourceFile, destPath);
            }
            else
            {
                Assert.Fail("The data for this test case has not been properly deployed! Missing file: " + destPath);
                return false;
            }
        }

        private bool CopyToNonExistantDirectory(FileCopier copier)
        {
            string destPath = Path.Combine(destDir, @"newdir\another\out.txt");
            return copier.TryCopyFile(sourceFile, destPath);
        }

        #endregion
    }
}
