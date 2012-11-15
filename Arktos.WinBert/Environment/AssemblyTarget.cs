namespace Arktos.WinBert.Environment
{
    using System;
    using System.Reflection;

    /// <summary>
    /// A thin wrapper on top of a reflection assembly.
    /// </summary>
    public sealed class AssemblyTarget : IAssemblyTarget
    {
        #region Fields & Constants

        private readonly Assembly assembly;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the AssemblyTarget class.
        /// </summary>
        /// <param name="assembly">
        /// The assembly.
        /// </param>
        public AssemblyTarget(Assembly assembly)
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
