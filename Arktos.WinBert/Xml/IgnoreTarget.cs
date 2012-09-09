namespace Arktos.WinBert.Xml
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public partial class IgnoreTarget
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the IgnoreTarget class.
        /// </summary>
        public IgnoreTarget()
        {
        }

        /// <summary>
        /// Initializes a new instance of the IgnoreTarget class.
        /// </summary>
        /// <param name="type">
        /// The type for the new IgnoreTarget.
        /// </param>
        /// <param name="name">
        /// The name for the new IgnoreTarget.
        /// </param>
        public IgnoreTarget(IgnoreType type, string name)
        {
            this.Type = type;
            this.Name = name;
        }

        #endregion
    }
}