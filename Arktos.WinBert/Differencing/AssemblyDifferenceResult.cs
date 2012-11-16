namespace Arktos.WinBert.Differencing
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Arktos.WinBert.Environment;
    using Microsoft.Cci;

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
        public AssemblyDifferenceResult(IAssemblyTarget oldAssemblyTarget, IAssemblyTarget newAssemblyTarget)
        {
            if (oldAssemblyTarget == null)
            {
                throw new ArgumentNullException("oldAssemblyTarget");
            }

            if (newAssemblyTarget == null)
            {
                throw new ArgumentNullException("newAssemblyTarget");
            }

            this.OldAssembly = oldAssemblyTarget;
            this.NewAssembly = newAssemblyTarget;
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
        public IAssemblyTarget NewAssembly { get; private set; }

        /// <inheritdoc />
        public IAssemblyTarget OldAssembly { get; private set; }

        /// <inheritdoc />
        public IList<ITypeDifferenceResult> TypeDifferences { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new AssemblyDifferenceResult from the target assemblies.
        /// </summary>
        /// <param name="oldAssembly">
        /// The old assembly.
        /// </param>
        /// <param name="newAssembly">
        /// The new assembly.
        /// </param>
        /// <returns>
        /// A new AssemblyDifferenceResult.
        /// </returns>
        public static AssemblyDifferenceResult Create(Assembly oldAssembly, Assembly newAssembly)
        {
            if (newAssembly == null)
            {
                throw new ArgumentNullException("newAssembly");
            }

            if (oldAssembly == null)
            {
                throw new ArgumentNullException("oldAssembly");
            }

            return new AssemblyDifferenceResult(new AssemblyTarget(oldAssembly), new AssemblyTarget(newAssembly));
        }

        /// <summary>
        /// Creates a new AssemblydifferenceResult from the target CCI assembly implementations.
        /// </summary>
        /// <param name="oldAssembly">
        /// The old assembly.
        /// </param>
        /// <param name="newAssembly">
        /// The new assembly.
        /// </param>
        /// <returns>
        /// A new AssemblyDifferenceResult.
        /// </returns>
        public static AssemblyDifferenceResult Create(IAssembly oldAssembly, IAssembly newAssembly)
        {
            if (newAssembly == null)
            {
                throw new ArgumentNullException("newAssembly");
            }

            if (oldAssembly == null)
            {
                throw new ArgumentNullException("oldAssembly");
            }

            return new AssemblyDifferenceResult(new MetadataAssemblyTarget(oldAssembly), new MetadataAssemblyTarget(newAssembly));
        }

        #endregion
    }
}