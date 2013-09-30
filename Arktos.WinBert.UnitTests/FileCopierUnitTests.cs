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

        private static readonly string TargetSrcDir = @"filecopier-test-files\";

        private static readonly string TargetDestDir = @"filecopier-test-files-copies\";

        private static readonly string TargetDirToCopy = TargetSrcDir + @"test-dir-to-copy\";

        private static readonly string TargetDirToCopySubdirName = "subdir";

        private static readonly string SrcFileName = @"src.txt";

        private static readonly string SrcFilePath = TargetSrcDir + SrcFileName;

        private static readonly string ExistingDestTxtName = @"existingDest.txt";

        private static readonly string ExistingDestTxtPath = TargetSrcDir + ExistingDestTxtName;

        private static readonly string ReadOnlyDestTxtName = @"readOnlyDest.txt";

        private static readonly string ReadOnlyDestTxtPath = TargetSrcDir + ReadOnlyDestTxtName;

        #endregion

        #region Test Plumbing

        [TestInitialize]
        public void TestInit()
        {
            try
            {
                if (!Directory.Exists(TargetDestDir))
                {
                    Directory.CreateDirectory(TargetDestDir);
                    File.Copy(ExistingDestTxtPath, TargetDestDir + ExistingDestTxtName);
                    File.Copy(ReadOnlyDestTxtPath, TargetDestDir + ReadOnlyDestTxtName);
                }
            }
            catch (Exception exc)
            {
                Assert.Inconclusive("Unable to set up test directory " + TargetDestDir + ". Exception: " + exc);
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            try
            {
                if (Directory.Exists(TargetDestDir))
                {
                    Directory.Delete(TargetDestDir, true);
                }
            }
            catch (Exception)
            {
                // Eat it.
            }
        }

        #endregion

        #region Test Methods

        #region TryCopyFile

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
            target.TryCopyFile(TargetDestDir, TargetDestDir);
        }

        #endregion

        #region CopyDirectory

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void CopyDirectory_NonExistantSourceDir()
        {
            var toCopy = Guid.NewGuid() + @"\";
            var target = new FileCopier();
            target.CopyDirectory(toCopy, TargetDestDir);
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void CopyDirectory_NonExistantDestDir_CreateFlagNotSet()
        {
            var srcDir = TargetSrcDir + @"\" + Guid.NewGuid();
            var target = new FileCopier();
            target.CopyDirectory(TargetDirToCopy, srcDir);
        }

        [TestMethod]
        public void CopyDirectory_CreateDestDirsNoOverwriteNoRecurse()
        {
            var srcDir = new DirectoryInfo(TargetDirToCopy);
            var uniqueFolder = Guid.NewGuid().ToString();
            var destDir = new DirectoryInfo(Path.Combine(TargetDestDir, uniqueFolder, srcDir.Name));
            var subDir = new DirectoryInfo(Path.Combine(TargetDestDir, uniqueFolder, srcDir.Name, TargetDirToCopySubdirName));

            var target = new FileCopier(FileCopierFlags.CreateDestinationDirectories);
            target.CopyDirectory(srcDir.FullName, destDir.FullName);

            Assert.IsTrue(destDir.Exists);
            Assert.IsTrue(File.Exists(Path.Combine(destDir.FullName, ExistingDestTxtName)));
            Assert.IsTrue(File.Exists(Path.Combine(destDir.FullName, SrcFileName)));
            Assert.IsTrue(File.Exists(Path.Combine(destDir.FullName, ReadOnlyDestTxtName)));

            Assert.IsFalse(subDir.Exists);
            Assert.IsFalse(File.Exists(Path.Combine(subDir.FullName, ExistingDestTxtName)));
            Assert.IsFalse(File.Exists(Path.Combine(subDir.FullName, SrcFileName)));
            Assert.IsFalse(File.Exists(Path.Combine(subDir.FullName, ReadOnlyDestTxtName)));
        }

        [TestMethod]
        public void CopyDirectory_CreateDestDirsNoOverwriteRecursive()
        {
            var srcDir = new DirectoryInfo(TargetDirToCopy);
            var uniqueFolder = Guid.NewGuid().ToString();
            var destDir = new DirectoryInfo(Path.Combine(TargetDestDir, uniqueFolder, srcDir.Name));
            var subDir = new DirectoryInfo(Path.Combine(TargetDestDir, uniqueFolder, srcDir.Name, TargetDirToCopySubdirName));

            var target = new FileCopier(FileCopierFlags.CreateDestinationDirectories);
            target.CopyDirectory(srcDir.FullName, destDir.FullName, true);

            Assert.IsTrue(destDir.Exists);
            Assert.IsTrue(File.Exists(Path.Combine(destDir.FullName, ExistingDestTxtName)));
            Assert.IsTrue(File.Exists(Path.Combine(destDir.FullName, SrcFileName)));
            Assert.IsTrue(File.Exists(Path.Combine(destDir.FullName, ReadOnlyDestTxtName)));

            Assert.IsTrue(subDir.Exists);
            Assert.IsTrue(subDir.GetFiles().Length == 1);
            Assert.IsFalse(File.Exists(Path.Combine(subDir.FullName, ExistingDestTxtName)));
            Assert.IsTrue(File.Exists(Path.Combine(subDir.FullName, SrcFileName)));
            Assert.IsFalse(File.Exists(Path.Combine(subDir.FullName, ReadOnlyDestTxtName)));
        }

        #endregion

        #endregion

        #region Private Methods

        private bool CopyToDirectory(FileCopier copier)
        {
            return copier.TryCopyFile(SrcFilePath, TargetDestDir);
        }

        private bool CopyFileToNamedTarget(FileCopier copier)
        {
            string destPath = Path.Combine(TargetDestDir, @"out.txt");
            return copier.TryCopyFile(SrcFilePath, destPath);
        }

        private bool CopyToExistingFile(FileCopier copier)
        {
            if (File.Exists(ExistingDestTxtPath))
            {
                return copier.TryCopyFile(SrcFilePath, ExistingDestTxtPath);
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

                return copier.TryCopyFile(SrcFilePath, ReadOnlyDestTxtPath);
            }
            else
            {
                Assert.Fail("The data for this test case has not been properly deployed! Missing file: " + ReadOnlyDestTxtPath);
                return false;
            }
        }

        private bool CopyToNonExistantDirectory(FileCopier copier)
        {
            string destPath = Path.Combine(TargetDestDir, @"newdir\another\out.txt");
            return copier.TryCopyFile(SrcFilePath, destPath);
        }

        #endregion
    }
}
