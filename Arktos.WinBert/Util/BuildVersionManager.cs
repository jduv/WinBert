namespace Arktos.WinBert.Util
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Arktos.WinBert.Xml;

    /// <summary>
    /// This simply class keeps a list of builds sorted by timestamp in a specified folder. As new builds are added 
    /// it will copy the contents of the target to the archive and store an entry in the sorted list. When the 
    /// BuildVersionManager reaches it's maximum capacity (defaulted to byte.MaxValue--or 255) it 
    /// will begin to drop the oldest build in favor for the new one. This in effect creates a moving window of 
    /// assemblies sorted by compile time. Only successful builds (obviously) will be archived.
    /// </summary>
    public sealed class BuildVersionManager
    {
        #region Constants & Fields

        /// <summary>
        /// Path to the archive.
        /// </summary>
        private readonly string archivePath;

        /// <summary>
        /// This is the hidden field holding the actual size for the max number of builds stored. Access this through 
        /// the public property only.
        /// </summary>
        private byte maxArchiveSize;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildVersionManager"/> class.
        /// </summary>
        /// <param name="archivePath">
        /// The path to where the caller wishes the archive to be constructed. Avoid relative paths here, as that will 
        /// likely get you in trouble during the save/load of each build. If this directory doesn't exist, then the archiver
        /// will create it.
        /// </param>
        /// <param name="maxBuildsArchivable">
        /// The maximum number of build history to keep. Lower numbers will result in lower disk space utilization.
        /// Defaults to max byte, or 255.
        /// </param>
        /// <param name="name">
        /// The name for this build archive. Defaults to unspecified (null)
        /// </param>
        public BuildVersionManager(string archivePath, byte maxBuildsArchivable = byte.MaxValue, string name = null)
        {
            if (string.IsNullOrEmpty(archivePath))
            {
                throw new ArgumentException("Invalid archive path! It cannot be null or empty.");
            }

            var fullPath = Path.GetFullPath(archivePath);

            // Files not allowed, must be a directory
            if (File.Exists(fullPath))
            {
                throw new ArgumentException("Invalid archive path! Must be a directory! Path: " + archivePath);
            }

            // If the directory doesn't exist, attempt to create it.
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            this.MaxArchiveSize = maxBuildsArchivable;
            this.SequenceNumber = 0;
            this.BuildArchive = new SortedList<uint, Build>();
            this.Name = name;
            this.archivePath = fullPath;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a path to the build archive.
        /// </summary>
        public string ArchivePath
        {
            get
            {
                if (string.IsNullOrEmpty(this.Name))
                {
                    return this.archivePath;
                }
                else
                {
                    return Path.Combine(this.archivePath, this.Name);
                }
            }
        }

        /// <summary>
        ///   Gets a sorted list that holds pointers to the builds as they are archived. The key is a numeric sequence 
        ///   number and the value is the path to the build assembly.
        /// </summary>
        public SortedList<uint, Build> BuildArchive { get; private set; }

        /// <summary>
        ///   Gets or sets the maximum archive size. Since this class is not designed to be a version control system and 
        ///   such functionality would be a waste of time and space for such a trivial application, the maximum allowed 
        ///   number of builds is stored as a byte value. Reducing the size of this value will cause the Manager to 
        ///   irreversibly drop the last n builds where n = previousSize - newSize.
        /// </summary>
        public byte MaxArchiveSize
        {
            get
            {
                return this.maxArchiveSize;
            }

            set
            {
                if (value < this.maxArchiveSize)
                {
                    this.maxArchiveSize = value;
                    this.TrimArchive();
                }
                else
                {
                    this.maxArchiveSize = value;
                }
            }
        }

        /// <summary>
        ///   Gets the name of this build archive. This value is used to create a separate folder for each build archive 
        ///   that shares a common archive path. If it is set, then the folder will be created. If not, then all builds will 
        ///   be copied to the master archive path specified in the archivePath field.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///   Gets the sequence number for sequential builds that are checked into the version manager.
        /// </summary>
        public uint SequenceNumber { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the target assembly to the builds archive. The path must be to a valid .NET compiled PE or assembly.
        /// It will be inserted in order into the sorted list, and it's assumed that this would be an incremental step from
        /// the previous add operation. This method will, however, not ensure that this implicit contract is satisfied.
        /// </summary>
        /// <param name="pathToSuccessfulBuild">
        /// The path to the assembly to add.
        /// </param>
        /// <returns>
        /// A copy of the build object that was added to the archive or null on failure.
        /// </returns>
        public Build AddNewSuccessfulBuild(string pathToSuccessfulBuild)
        {
            if (File.Exists(pathToSuccessfulBuild))
            {
                var copier =
                    new FileCopier(
                        FileCopierFlags.AlwaysOverwriteDestination | FileCopierFlags.CreateDestinationDirectories);

                var pathInArchive = this.GetArchivePath(pathToSuccessfulBuild);
                if (copier.TryCopyFile(pathToSuccessfulBuild, pathInArchive))
                {
                    if (this.BuildArchive.Count == this.MaxArchiveSize)
                    {
                        var lastElement = this.BuildArchive.Last();
                        try
                        {
                            File.Delete(lastElement.Value.Path);
                        }
                        catch (Exception)
                        {
                                // Unable to remove the last build in the archive list from disk.
                        }
                        finally
                        {
                            this.BuildArchive.Remove(lastElement.Key);
                        }
                    }

                    return this.LoadBuild(this.SequenceNumber, pathInArchive);
                }
            }

            throw new ArgumentException("The target assembly for the build doesn't exist!");            
        }

        /// <summary>
        /// Returns a path to the build with the target sequence number. If the target sequence number doesn't 
        /// exist in the archive the method will return null.
        /// </summary>
        /// <param name="sequenceNumber">
        /// The sequence number to fetch
        /// </param>
        /// <returns>
        /// A Build object holding the path to the target build.
        /// </returns>
        public Build GetBuildRevision(uint sequenceNumber)
        {
            Build build = null;
            if (this.BuildArchive.TryGetValue(sequenceNumber, out build))
            {
                return build;
            }

            return null;
        }

        /// <summary>
        /// Grabs the build preceding the target revision, regardless of sequence number (there could be gaps). 
        /// This method relies on the fact that everything is sorted inside the collection holding the builds.
        /// </summary>
        /// <param name="sequenceNumber">
        /// The sequence number to fetch the preceding build for.
        /// </param>
        /// <returns>
        /// A build preceding the target sequence number, or null if one doesn't exist. Passing this method zero, for
        /// example, will return null because there will never be a build before the zeroth.
        /// </returns>
        public Build GetBuildRevisionPreceding(uint sequenceNumber)
        {
            if (this.BuildArchive.Count > 1)
            {
                int index = this.BuildArchive.IndexOfKey(sequenceNumber);
                return this.BuildArchive.ElementAt(index - 1).Value;                    
            }

            return null;
        }

        /// <summary>
        /// Returns a path to the most recently built assembly according to the archive.
        /// </summary>
        /// <returns>
        /// A path to the most recently built assembly according to the archive.
        /// </returns>
        public Build GetMostRecentBuild()
        {
            if (this.BuildArchive.Count > 0)
            {
                var mostRecent = this.BuildArchive.Last();
                return mostRecent.Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Call this method when loading a build archive from configuration. It will not copy any files, but instead 
        /// rely on the caller to ensure that the file is properly there. If the file path passed to the method doesn't
        /// exist, then it will throw an exception. It will also adjust the internal sequence number to be one plus the 
        /// maximum sequence number passed to the instance in it's lifetime.
        /// </summary>
        /// <param name="sequenceNumber">
        /// The sequence number to add the build at.
        /// </param>
        /// <param name="path">
        /// The path of the file to add. If this doesn't exist, expect an ArgumentException.
        /// </param>
        /// <returns>
        /// A copy of the build object that was added to the archive or null on failure.
        /// </returns>
        public Build LoadBuild(uint sequenceNumber, string path)
        {
            if (sequenceNumber == uint.MaxValue)
            {
                var error =
                    string.Format(
                        "Potential integer overflow detected! The passed in sequence number is too large! {0}",
                        sequenceNumber);
                throw new ArgumentException(error);
            }

            if (File.Exists(path))
            {
                // adjust the sequence number if appropriate. This could overflow.
                if (sequenceNumber >= this.SequenceNumber)
                {
                    this.SequenceNumber = sequenceNumber + 1;
                }

                var b = new Build(sequenceNumber, path);
                this.BuildArchive.Add(sequenceNumber, b);

                return b;
            }
            else
            {
                var error = string.Format(
                    "The path {0} doesn't exist! Unable to load this file into the archive!", path);
                throw new ArgumentException(error);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Converts the target path to an in-archive path.
        /// </summary>
        /// <param name="buildPath">
        /// The path to convert.
        /// </param>
        /// <returns>
        /// Returns the archive path for the given build path.
        /// </returns>
        private string GetArchivePath(string buildPath)
        {
            string fileName = Path.GetFileName(buildPath);

            if (string.IsNullOrEmpty(this.Name))
            {
                return Path.Combine(this.archivePath, this.SequenceNumber.ToString(), fileName);
            }

            return Path.Combine(this.archivePath, this.Name, this.SequenceNumber.ToString(), fileName);
        }

        /// <summary>
        /// This method prunes the sorted list to within the bounds of the current maxArchiveSize field. This method
        ///   will be destructive and cause data loss if the current list size is greater than maxArchiveSize.
        /// </summary>
        private void TrimArchive()
        {
            if (this.MaxArchiveSize == 0)
            {
                this.BuildArchive.Clear();
            }
            else if (this.BuildArchive.Count > this.MaxArchiveSize)
            {
                int numToRemove = this.BuildArchive.Count - this.MaxArchiveSize;
                for (int i = 0; i < numToRemove; i++)
                {
                    this.BuildArchive.RemoveAt(i);
                }
            }
        }

        #endregion
    }
}