namespace Arktos.WinBert.Environment
{
    using System;

    /// <summary>
    /// Defines behavior for implementations that load assemblies into a built-in application domain.
    /// </summary>
    public interface IAppDomainContext : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the contained application domain.
        /// </summary>
        AppDomain Domain { get; }

        /// <summary>
        /// Gets the assembly resolver responsible for resolving assemblies in the application domain.
        /// </summary>
        IAssemblyResolver Resolver { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the assembly target into the application domain contained by this environment.
        /// </summary>
        /// <param name="loadMethod">
        /// The LoadMethod to use when importing the assembly.
        /// </param>
        /// <param name="target">
        /// The assembly target to load.
        /// </param>
        /// <returns>
        /// An assembly target.
        /// </returns>
        AssemblyTarget LoadTarget(LoadMethod loadMethod, AssemblyTarget target);

        /// <summary>
        /// Loads the assembly at the specified path into the current application domain.
        /// </summary>
        /// <param name="loadMethod">
        /// The LoadMethod to use when importing the assembly.
        /// </param>
        /// <param name="path">
        /// The path to the assembly.
        /// </param>
        /// <param name="pdbPath">
        /// The path to the assembly's PDB file.
        /// </param>
        /// <returns>
        /// An assembly target.
        /// </returns>
        AssemblyTarget LoadAssembly(LoadMethod loadMethod, string path, string pdbPath = null);

        #endregion
    }
}
