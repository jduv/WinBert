namespace Arktos.WinBert.Environment
{
    using System;
    using Microsoft.Cci;

    /// <summary>
    /// A thin wrapper on top of a CCI metadata assembly.
    /// </summary>
    public sealed class MetadataAssemblyTarget : IAssemblyTarget
    {
        #region Fields & Constants

        private readonly IAssembly assembly;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the MetadataAssemblyTarget class.
        /// </summary>
        /// <param name="assembly">
        /// The assembly.
        /// </param>
        public MetadataAssemblyTarget(IAssembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            this.assembly = assembly;
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public string Location
        {
            get
            {
                return this.assembly.Location;
            }
        }

        #endregion
    }
}
