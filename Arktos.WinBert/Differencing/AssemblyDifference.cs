﻿namespace Arktos.WinBert.Differencing
{
    using AppDomainToolkit;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// This class represents a difference result between two assemblies. This class should always be marked as
    /// serializable to avoid issues with app domain lifecycles. Never pass the raw assemblies, however back and
    /// forth across application domains--you'll pollute the current app domain.
    /// </summary>
    [Serializable]
    public sealed class AssemblyDifference : IAssemblyDifference
    {
        #region Fields & Constructors

        public IDictionary<string, ITypeDifference> typeDiffLookup;

        #endregion

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
        /// <param name="itemsCompared">
        /// The number of items compared.
        /// </param>
        /// <param name="typeDiffs">
        /// A list of type difference.
        /// </param>
        public AssemblyDifference(Assembly oldAssembly, Assembly newAssembly, IList<ITypeDifference> typeDiffs)
        {
            if (oldAssembly == null)
            {
                throw new ArgumentNullException("oldAssembly");
            }

            if (newAssembly == null)
            {
                throw new ArgumentNullException("newAssembly");
            }

            this.OldAssemblyTarget = AssemblyTarget.FromAssembly(oldAssembly);
            this.NewAssemblyTarget = AssemblyTarget.FromAssembly(newAssembly);
            this.TypeDifferences = typeDiffs ?? Enumerable.Empty<ITypeDifference>();
            this.typeDiffLookup = this.TypeDifferences.ToDictionary(x => x.FullName);
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public bool AreDifferences
        {
            get
            {
                return this.TypeDifferences.Count() > 0;
            }
        }

        /// <inheritdoc />
        public IAssemblyTarget NewAssemblyTarget { get; private set; }

        /// <inheritdoc />
        public IAssemblyTarget OldAssemblyTarget { get; private set; }

        /// <inheritdoc />
        public IEnumerable<ITypeDifference> TypeDifferences { get; private set; }

        /// <inheritdoc />
        public ITypeDifference this[string fullName]
        {
            get
            {
                return this.typeDiffLookup[fullName];
            }
        }

        #endregion
    }
}