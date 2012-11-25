namespace Arktos.WinBert.Xml
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public partial class Build : IEquatable<Build>
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
        /// <param name="AssemblyPath">
        /// The AssemblyPath to the target.
        /// </param>
        public Build(uint sequenceNumber, string AssemblyPath)
        {
            this.SequenceNumber = sequenceNumber;
            this.AssemblyPath = AssemblyPath;
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
            return this.SequenceNumber == build.SequenceNumber && this.AssemblyPath.Equals(build.AssemblyPath);
        }

        /// <summary>
        /// Override of GetHashCode for uniqueness.
        /// </summary>
        /// <returns>
        /// A semi-unique hash code for this object.
        /// </returns>
        public override int GetHashCode()
        {
            return this.SequenceNumber.GetHashCode() ^ this.AssemblyPath.GetHashCode();
        }

        #endregion
    }
}