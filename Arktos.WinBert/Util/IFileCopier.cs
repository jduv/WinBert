namespace Arktos.WinBert.Util
{
    /// <summary>
    /// Implementations should simply have the ability to copy files.
    /// </summary>
    public interface IFileCopier
    {
        /// <summary>
        /// Attempts to copy the contents of the <paramref name="sourcePath"/> to the <paramref name="destPath"/>.
        /// </summary>
        /// <param name="sourcePath">
        /// The source path.
        /// </param>
        /// <param name="destPath">
        /// The destination path.
        /// </param>
        /// <param name="recurse">
        /// Should the copy be recursive?
        /// </param>
        /// <returns>
        /// True if the copy was successful, false otherwise.
        /// </returns>
        void CopyDirectory(string sourcePath, string destPath, bool recurse = false);

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
        bool TryCopyFile(string sourcePath, string destPath);
    }
}
