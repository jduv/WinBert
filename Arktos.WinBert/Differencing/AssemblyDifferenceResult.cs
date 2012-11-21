namespace Arktos.WinBert.Differencing
{
    using System;
    using System.Collections.Generic;
    using Arktos.WinBert.Environment;
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
            if (oldAssembly == null)
            {
                throw new ArgumentNullException("oldAssembly");
            }

            if (newAssembly == null)
            {
                throw new ArgumentNullException("newAssembly");
            }

            this.OldAssembly = oldAssembly;
            this.OldAssemblyTarget = AssemblyTarget.Create(oldAssembly);
            this.NewAssembly = newAssembly;
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

        /// <summary>
        /// Gets the new assembly.
        /// </summary>
        public Assembly NewAssembly { get; private set; }

        /// <inheritdoc />
        public AssemblyTarget NewAssemblyTarget { get; private set; }

        /// <summary>
        /// Gets the old assembly.
        /// </summary>
        public Assembly OldAssembly { get; private set; }

        /// <inheritdoc />
        public AssemblyTarget OldAssemblyTarget { get; private set; }

        /// <inheritdoc />
        public IList<ITypeDifferenceResult> TypeDifferences { get; private set; }

        #endregion
    }
}