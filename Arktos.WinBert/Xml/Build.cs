namespace Arktos.WinBert.Xml
{
    /// <summary>
    /// Partial implementation of a auto generated class. This implementation adds some basic
    ///   functionality for convenience only.
    /// </summary>
    public partial class Build
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the Build class.
        /// </summary>
        public Build()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Build class.
        /// </summary>
        /// <param name="sequenceNumber">
        /// The sequence number for the build.
        /// </param>
        /// <param name="path">
        /// The path to the target.
        /// </param>
        public Build(uint sequenceNumber, string path)
        {
            this.SequenceNumber = sequenceNumber;
            this.Path = path;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Override of Equals to make comparisons easier.
        /// </summary>
        /// <param name="obj">
        /// The object to compare ourselves to.
        /// </param>
        /// <returns>
        /// True if the objects are equal, false otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Build)
            {
                return this.Equals(obj as Build);
            }

            return false;
        }

        /// <summary>
        /// Compares this build to the target build object.
        /// </summary>
        /// <param name="build">
        /// The build to compare to.
        /// </param>
        /// <returns>
        /// True if the builds are equal, false otherwise.
        /// </returns>
        public bool Equals(Build build)
        {
            return this.SequenceNumber == build.SequenceNumber && this.Path.Equals(build.Path);
        }

        /// <summary>
        /// Override of GetHashCode for uniqueness.
        /// </summary>
        /// <returns>
        /// A semi-unique hash code for this object.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.SequenceNumber.GetHashCode() ^ this.Path.GetHashCode();
        }

        #endregion
    }
}