namespace Arktos.WinBert.Differencing
{
    using System;
    using System.Collections.Generic;
    using Arktos.WinBert.Environment;
    using System.Reflection;

    /// <summary>
    /// This class represents a difference result between two assemblies. This class should always be marked as
    /// serializable to avoid issues with app domain lifecycles. Never pass the raw assemblies, however back and
    /// forth across application domains--you'll pollute the current app domain.
    /// </summary>
    [Serializable]
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
            if (oldAssembly == null)
            {
                throw new ArgumentNullException("oldAssembly");
            }

            if (newAssembly == null)
            {
                throw new ArgumentNullException("newAssembly");
            }

            this.OldAssemblyTarget = AssemblyTarget.Create(oldAssembly);
            this.NewAssemblyTarget = AssemblyTarget.Create(newAssembly);
            this.TypeDifferences = new List<ITypeDifferenceResult>();
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public bool IsDifferent
        {
            get
            {
                return this.TypeDifferences.Count > 0;
            }
        }

        /// <inheritdoc />
        public IAssemblyTarget NewAssemblyTarget { get; private set; }

        /// <inheritdoc />
        public IAssemblyTarget OldAssemblyTarget { get; private set; }

        /// <inheritdoc />
        public IList<ITypeDifferenceResult> TypeDifferences { get; private set; }

        #endregion
    }
}