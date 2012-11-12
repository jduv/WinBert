namespace Arktos.WinBert.Util
{
    using System;
    using System.IO;

    /// <summary>
    /// These flags will define how the FileCopier object behaves in certain situations.
    /// </summary>
    [Flags]
    public enum FileCopierFlags
    {
        /// <summary>
        ///   This flag tells the copier to always overwrite the destination file if it can. This results in the proper
        ///   core API's being used whenever the copy methods are used. This is not a catch all flag that enables
        ///   the copier to overwrite locked or read only files, there are other flags for that. However specify this flag
        ///   if you wish to overwrite an existing destination file on copy if the file clearly exists, and
        ///   isn't read only.
        /// </summary>
        AlwaysOverwriteDestination = 1 << 0, 

        /// <summary>
        ///   This flag tells the copier to attempt to overwrite a read only file. This flag still does not guarantee
        ///   a successful overwrite.
        /// </summary>
        AttemptOverwriteReadonlyDest = 1 << 1, 

        /// <summary>
        ///   This flag tells the copier to create all directories that do not exist in the destination file path. For
        ///   example if copying a file to C:\foo\bar\baz\myfile.txt and the bar and baz directories don't exist, the
        ///   copier will create them for you.
        /// </summary>
        CreateDestinationDirectories = 1 << 2
    }

    /// <summary>
    /// This class has the ability to copy files. It uses the standard core libraries, but
    ///   provides some extra functionality based on the bitmask passed to the constructor. See
    ///   static members for more information about the features of this class. This class will only
    ///   copy files, no directories.
    /// </summary>
    public sealed class FileCopier
    {
        #region Constants and Fields

        /// <summary>
        ///   The flags for this file copier.
        /// </summary>
        private readonly FileCopierFlags flags;

        #endregion
         
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the FileCopier class. No flags are set, so the copier will never
        /// overwrite destination files and fail if any directories in the destination path don't exist.
        /// </summary>
        public FileCopier()
        {
            this.flags = 0;
        }
        
        /// <summary>
        /// Initializes a new instance of the FileCopier class. Set flags for this object based on the public static
        /// fields on it.
        /// </summary>
        /// <param name="flags">
        /// Flags for this file copier.
        /// </param>
        public FileCopier(FileCopierFlags flags)
        {
            this.flags = flags;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Copies a file given by the source path to the destination path. This method will not throw exceptions upon
        /// errors, but instead return a boolean indicating if the copy was successful or not. If the destination path
        /// is an existing directory, then this method will attempt to infer the file name.
        /// </summary>
        /// <param name="sourcePath">
        /// The source path.
        /// </param>
        /// <param name="destPath">
        /// The destination path.
        /// </param>
        /// <returns>
        /// True on a successful copy, false otherwise.
        /// </returns>
        public bool TryCopyFile(string sourcePath, string destPath)
        {
            // check to see if source is a directory
            if (Directory.Exists(sourcePath))
            {
                throw new ArgumentException("The source path cannot be a directory! Path: " + sourcePath);
            }

            // make sure the source file exists
            if (File.Exists(sourcePath))
            {
                // if the destination file exists
                if (File.Exists(destPath))
                {
                    // overwrite it if we can
                    return this.OverwriteFile(sourcePath, destPath);
                }

                if (Directory.Exists(destPath))
                {
                    // else if the destination is an existing directory, infer the file path and copy
                    var inferredDestFilePath = this.InferDestFilePath(sourcePath, destPath);
                    return this.CopyToNewFile(sourcePath, inferredDestFilePath);
                }

                // in this case, the file name doesn't exist (e.g. src: in.txt dst: out.txt)
                // or we have some unknown directories in the output path (e.g. src: in.txt, dst: foo/bar/)

                // check for a file name
                string destFileName = Path.GetFileName(destPath);

                // check for a directory path
                string directoryPath = Path.GetDirectoryName(destPath);

                if (string.IsNullOrEmpty(destFileName) || 
                    !(string.IsNullOrEmpty(directoryPath) || Directory.Exists(directoryPath)))
                {
                    // No file name means we have a directory structure to create 
                    return this.CopyToNewDirectory(sourcePath, destPath);
                }

                // we have been given a straight file name (relative), copy away
                return this.CopyToNewFile(sourcePath, destPath);
            }

            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Copies the source path to the destination path, creating any non-existant directories on the way.
        /// </summary>
        /// <param name="sourcePath">
        /// The source path.
        /// </param>
        /// <param name="destPath">
        /// The destination path.
        /// </param>
        /// <returns>
        /// True if the copy is successful, false otherwise.
        /// </returns>
        private bool CopyToNewDirectory(string sourcePath, string destPath)
        {
            try
            {
                if ((this.flags & FileCopierFlags.CreateDestinationDirectories)
                    == FileCopierFlags.CreateDestinationDirectories)
                {
                    string directoryString = Path.GetDirectoryName(destPath);
                    Directory.CreateDirectory(directoryString);

                    if (Directory.Exists(destPath))
                    {
                        string inferredDestFilePath = this.InferDestFilePath(sourcePath, destPath);
                        File.Copy(sourcePath, inferredDestFilePath);
                    }
                    else
                    {
                        File.Copy(sourcePath, destPath);
                    }

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Copies the source file to the destination path. It is assumed that the destPath is an actual file name
        ///   and not a directory name. This method will throw an exception if this is not true.
        /// </summary>
        /// <param name="sourcePath">
        /// The path to the source file to copy.
        /// </param>
        /// <param name="destPath">
        /// The destination file name to copy to.
        /// </param>
        /// <returns>
        /// True if the copy is successful, false otherwise.
        /// </returns>
        private bool CopyToNewFile(string sourcePath, string destPath)
        {
            try
            {
                File.Copy(sourcePath, destPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// This method will take a source path (example: C:\test\foo.txt) and a destination path (example: C:\test\hello\) 
        /// and infer from the source what the destination should be (e.g. C:\test\hello\foo.txt). This method isn't 
        /// incredibly smart, so it should be used with caution.
        /// </summary>
        /// <example>
        /// Example 1:
        /// InferDestFilePath("C:\test.txt", "C:\directory\") =&gt; "C:\directory\test.txt"
        /// Example 2:
        /// InferDestFilePath("C:\text.txt", "C:\out.txt") =&gt; "C:\out.txt"
        /// </example>
        /// <param name="sourcePath">
        /// The source path.
        /// </param>
        /// <param name="destPath">
        /// The destination path.
        /// </param>
        /// <returns>
        /// A path that represents what we think the true destination file path should be based
        /// on what the source path looks like.
        /// </returns>
        private string InferDestFilePath(string sourcePath, string destPath)
        {
            string sourceFileName = Path.GetFileName(sourcePath);
            string destFileName = Path.GetFileName(destPath);

            if (string.IsNullOrEmpty(sourceFileName) || !string.IsNullOrEmpty(destFileName))
            {
                // no file name, the source is a directory. No idea what the destination path should be. Just return it.
                return destPath;
            }

            // copying to a directory, slap the file name on there from the source file
            return Path.Combine(destPath, sourceFileName);
        }

        /// <summary>
        /// Copies the source file to the destination file. This will attempt to overwrite the destination file depending 
        ///   on if the proper flags were set on the construction of this FileCopier object.
        /// </summary>
        /// <param name="sourcePath">
        /// The source path.
        /// </param>
        /// <param name="destPath">
        /// The destination path.
        /// </param>
        /// <returns>
        /// True if the copy is successful, false otherwise.
        /// </returns>
        private bool OverwriteFile(string sourcePath, string destPath)
        {
            if ((this.flags & FileCopierFlags.AlwaysOverwriteDestination) == FileCopierFlags.AlwaysOverwriteDestination)
            {
                var info = new FileInfo(destPath);

                if (info.IsReadOnly)
                {
                    if ((this.flags & FileCopierFlags.AttemptOverwriteReadonlyDest)
                        == FileCopierFlags.AttemptOverwriteReadonlyDest)
                    {
                        info.IsReadOnly = false;
                    }
                    else
                    {
                        return false;
                    }
                }

                try
                {
                    File.Copy(sourcePath, destPath, true);
                    return true;
                }
                catch (Exception)
                {
                    // Fall through
                }
            }

            return false;
        }

        #endregion
    }
}