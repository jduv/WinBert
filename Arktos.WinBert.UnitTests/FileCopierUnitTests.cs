namespace Arktos.WinBert.UnitTests
{
    using System;
    using System.IO;
    using Arktos.WinBert.Util;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [DeploymentItem("filecopier-test-files", "filecopier-test-files")]
    public class FileCopierUnitTests
    {
        #region Fields and Constants

        private static readonly string SrcDir = @"filecopier-test-files\";

        private static readonly string DestDir = @"filecopier-test-files-copies\";

        private static readonly string SrcFile = SrcDir + @"src.txt";

        private static readonly string ExistingDestTxtName = @"existingDest.txt";

        private static readonly string ExistingDestTxtPath = SrcDir + ExistingDestTxtName;

        private static readonly string ReadOnlyDestTxtName = @"readOnlyDest.txt";

        private static readonly string ReadOnlyDestTxtPath = SrcDir + ReadOnlyDestTxtName;

        #endregion

        #region Test Plumbing

        [TestInitialize]
        public void TestInit()
        {
            try
            {
                if (!Directory.Exists(DestDir))
                {
                    Directory.CreateDirectory(DestDir);
                    File.Copy(ExistingDestTxtPath, DestDir + ExistingDestTxtName);
                    File.Copy(ReadOnlyDestTxtPath, DestDir + ReadOnlyDestTxtName);
                }
            }
            catch (Exception exc)
            {
                Assert.Inconclusive("Unable to set up test directory " + DestDir + ". Exception: " + exc);
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            try
            {
                if (Directory.Exists(DestDir))
                {
                    Directory.Delete(DestDir, true);
                }
            }
            catch (Exception)
            {
                // Eat it.
            }
        }

        #endregion

        #region Test Methods

        [TestMethod]
        public void TryCopyFile_NoFlags()
        {
            var target = new FileCopier();

            Assert.IsTrue(this.CopyToDirectory(target));
            Assert.IsTrue(this.CopyFileToNamedTarget(target));
            Assert.IsFalse(this.CopyToExistingFile(target));
            Assert.IsFalse(this.CopyToReadOnlyFile(target));
            Assert.IsFalse(this.CopyToNonExistantDirectory(target));
        }

        [TestMethod]
        public void TryCopyFile_AlwaysOverwriteDest()
        {
            var target = new FileCopier(FileCopierFlags.AlwaysOverwriteDestination);

            Assert.IsTrue(this.CopyToDirectory(target));
            Assert.IsTrue(this.CopyFileToNamedTarget(target));
            Assert.IsTrue(this.CopyToExistingFile(target));
            Assert.IsFalse(this.CopyToReadOnlyFile(target));
            Assert.IsFalse(this.CopyToNonExistantDirectory(target));
        }

        [TestMethod]
        public void TryCopyFile_AlwaysOverwriteDestWithReadonly()
        {
            var target = new FileCopier(FileCopierFlags.AlwaysOverwriteDestination |
                                                      FileCopierFlags.AttemptOverwriteReadonlyDest);

            Assert.IsTrue(this.CopyToDirectory(target));
            Assert.IsTrue(this.CopyFileToNamedTarget(target));
            Assert.IsTrue(this.CopyToExistingFile(target));
            Assert.IsTrue(this.CopyToReadOnlyFile(target));
            Assert.IsFalse(this.CopyToNonExistantDirectory(target));
        }

        [TestMethod]
        public void TryCopyFile_CreateDestDirs()
        {
            var target = new FileCopier(FileCopierFlags.CreateDestinationDirectories);

            Assert.IsTrue(this.CopyToDirectory(target));
            Assert.IsTrue(this.CopyFileToNamedTarget(target));
            Assert.IsFalse(this.CopyToExistingFile(target));
            Assert.IsFalse(this.CopyToReadOnlyFile(target));
            Assert.IsTrue(this.CopyToNonExistantDirectory(target));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TryCopyFile_SourceDir()
        {
            var target = new FileCopier();
            target.TryCopyFile(DestDir, DestDir);
        }

        #endregion

        #region Private Methods

        private bool CopyToDirectory(FileCopier copier)
        {
            return copier.TryCopyFile(SrcFile, DestDir);
        }

        private bool CopyFileToNamedTarget(FileCopier copier)
        {
            string destPath = Path.Combine(DestDir, @"out.txt");
            return copier.TryCopyFile(SrcFile, destPath);
        }

        private bool CopyToExistingFile(FileCopier copier)
        {            
            if (File.Exists(ExistingDestTxtPath))
            {
                return copier.TryCopyFile(SrcFile, ExistingDestTxtPath);
            }
            else
            {
                Assert.Fail("The data for this test case has not been properly deployed! Missing file: " + ExistingDestTxtPath);
                return false;
            }
        }

        private bool CopyToReadOnlyFile(FileCopier copier)
        {            
            if (File.Exists(ReadOnlyDestTxtPath))
            {
                FileInfo info = new FileInfo(ReadOnlyDestTxtPath);

                // ensure that the file is readonly.
                if (!info.IsReadOnly)
                {
                    info.IsReadOnly = true;
                }

                return copier.TryCopyFile(SrcFile, ReadOnlyDestTxtPath);
            }
            else
            {
                Assert.Fail("The data for this test case has not been properly deployed! Missing file: " + ReadOnlyDestTxtPath);
                return false;
            }
        }

        private bool CopyToNonExistantDirectory(FileCopier copier)
        {
            string destPath = Path.Combine(DestDir, @"newdir\another\out.txt");
            return copier.TryCopyFile(SrcFile, destPath);
        }

        #endregion
    }
}
