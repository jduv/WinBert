namespace WinBert.Differencing
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// This class represents a difference result between two assemblies.
    /// </summary>
    public sealed class AssemblyDifferenceResult : IAssemblyDifferenceResult
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the AssemblyDifferenceResult class.
        /// </summary>
        /// <param name="oldAssembly">
        /// The old assembly.
        /// </param>
        /// <param name="newAssembly">
        /// The new assembly.
        /// </param>
        public AssemblyDifferenceResult(Assembly oldAssembly, Assembly newAssembly)
        {
            this.OldObject = oldAssembly;
            this.NewObject = newAssembly;
            this.TypeDifferences = new List<ITypeDifferenceResult>();
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether there is a difference or not.
        /// </summary>
        public bool DifferenceResult
        {
            get
            {
                return this.TypeDifferences.Count > 0 || this.NewObject == null || this.OldObject == null;
            }
        }

        /// <summary>
        ///   Gets the new assembly.
        /// </summary>
        public Assembly NewObject { get; private set; }

        /// <summary>
        ///   Gets the old assembly.
        /// </summary>
        public Assembly OldObject { get; private set; }

        /// <summary>
        ///   Gets a list of type differences.
        /// </summary>
        public IList<ITypeDifferenceResult> TypeDifferences { get; private set; }

        #endregion
    }
}