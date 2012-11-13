namespace Arktos.WinBert.Differencing
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// This class represents a difference result between two assemblies.
    /// </summary>
    public sealed class AssemblyDifferenceResult : IAssemblyDifferenceResult
    {
        #region Constructors & Destructors

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

        /// <inheritdoc />
        public bool IsDifferent
        {
            get
            {
                return this.TypeDifferences.Count > 0 || this.NewObject == null || this.OldObject == null;
            }
        }

        /// <inheritdoc />
        public Assembly NewObject { get; private set; }

        /// <inheritdoc />
        public Assembly OldObject { get; private set; }

        /// <inheritdoc />
        public IList<ITypeDifferenceResult> TypeDifferences { get; private set; }

        #endregion
    }
}