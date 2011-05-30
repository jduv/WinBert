namespace WinBertUnitTests
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WinBert.Util;

    /// <summary>
    /// Tests the FileCopier class
    /// </summary>
    [TestClass]
    public class FileCopierTest
    {
        /// <summary>
        ///   Destination directory for all copy operations
        /// </summary>
        private static readonly string destDir = @"filecopier-test-files-deploy\";

        /// <summary>
        ///   Source file name
        /// </summary>
        private static readonly string sourceFile = @"src.txt";

        /// <summary>
        ///   Existing destination file name
        /// </summary>
        private static readonly string existingDestTxt = "existingDest.txt";

        /// <summary>
        ///   Read only destination file name.
        /// </summary>
        private static readonly string readOnlyDestTxt = "readOnlyDest.txt";

        /// <summary>
        ///   The FileCopier instance to test.
        /// </summary>
        private FileCopier fileCopierUnderTest = null;

        /// <summary>
        /// Handles initialization before each test.
        /// </summary>
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

        /// <summary>
        /// Performs any cleanup after each test.
        /// </summary>
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

        /// <summary>
        /// Tests a file copier with no flags set
        /// </summary>
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

        /// <summary>
        /// Tests the ALWAYS_OVERWRITE_DESTINATION flag on the FileCopier class.
        /// </summary>
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

        /// <summary>
        /// Tests the ATTEMPT_OVERWRIGE_READONLY_DEST flag on the FileCopier class.
        /// </summary>
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

        /// <summary>
        /// Tests the CREATE_DEST_DIRECTORIES flag on the FileCopier class.
        /// </summary>
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

        /// <summary>
        /// Tests copying a source directory. This should fail with an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCopySourceDirectory()
        {
            this.fileCopierUnderTest = new FileCopier();
            this.fileCopierUnderTest.TryCopyFile(destDir, destDir);
        }

        /// <summary>
        /// Copies a file.
        /// </summary>
        /// <param name="copier">
        /// The file copier to test
        /// </param>
        /// <returns>
        /// The return result of copier.TryCopyFile(src, dest)
        /// </returns>
        private bool CopyToDirectory(FileCopier copier)
        {
            return copier.TryCopyFile(sourceFile, destDir);
        }

        /// <summary>
        /// Copies a file to a named target that doesn't (shouldn't) exist.
        /// </summary>
        /// <param name="copier">
        /// The file copier to test
        /// </param>
        /// <returns>
        /// The return result of copier.TryCopyFile(src, dest)
        /// </returns>
        private bool CopyFileToNamedTarget(FileCopier copier)
        {
            string destPath = Path.Combine(destDir, @"out.txt");

            return copier.TryCopyFile(sourceFile, destPath);
        }

        /// <summary>
        /// Copies a file to another handle that already exists.
        /// </summary>
        /// <param name="copier">
        /// The file copier to test
        /// </param>
        /// <returns>
        /// The return result of copier.TryCopyFile(src, dest)
        /// </returns>
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

        /// <summary>
        /// Copies a file to another handle that is set to read only.
        /// </summary>
        /// <param name="copier">
        /// The file copier to test
        /// </param>
        /// <returns>
        /// The return result of copier.TryCopyFile(src, dest)
        /// </returns>
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

        /// <summary>
        /// Copies a file to a directory that doesn't exist.
        /// </summary>
        /// <param name="copier">
        /// The file copier to test
        /// </param>
        /// <returns>
        /// The return result of copier.TryCopyFile(src, dest)
        /// </returns>
        private bool CopyToNonExistantDirectory(FileCopier copier)
        {
            string destPath = Path.Combine(destDir, @"newdir\another\out.txt");

            return copier.TryCopyFile(sourceFile, destPath);
        }
    }
}
