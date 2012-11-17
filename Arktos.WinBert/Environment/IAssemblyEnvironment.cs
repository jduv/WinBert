namespace Arktos.WinBert.Environment
{
    using System;

    /// <summary>
    /// Defines behavior for implementations that load assemblies into a built-in application domain.
    /// </summary>
    public interface IAssemblyEnvironment : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the contained application domain.
        /// </summary>
        AppDomain Domain { get; }

        #endregion

        #region Methods

        /// <summary>
        /// This call should be the same as calling Assembly.LoadFile().
        /// </summary>
        /// <param name="path">
        /// The path to the assembly to load.
        /// </param>
        /// <returns>
        /// The target assembly.
        /// </returns>
        ILoadedAssemblyTarget LoadFile(string path);

        /// <summary>
        /// This call should be the same as calling Assembly.LoadFrom().
        /// </summary>
        /// <param name="path">
        /// The path to the assembly to load.
        /// </param>
        /// <returns>
        /// The target assembly.
        /// </returns>
        ILoadedAssemblyTarget LoadFrom(string path);

        /// <summary>
        /// Loads the bits of the assembly at the target path into the current application domain.
        /// </summary>
        /// <param name="path">
        /// The path to the assembly to load.
        /// </param>
        /// <returns>
        /// The target assembly.
        /// </returns>
        ILoadedAssemblyTarget LoadBits(string path);

        /// <summary>
        /// Loads the bits of the assembly at the target path into the current application domain along with
        /// it's PDB information.
        /// </summary>
        /// <param name="assemblyPath">
        /// The path to the assembly to load.
        /// </param>
        /// <param name="pdbPath">
        /// The path to the debug symbols for the target assembly.
        /// </param>
        /// <returns>
        /// The target assembly.
        /// </returns>
        ILoadedAssemblyTarget LoadBits(string assemblyPath, string pdbPath);

        /// <summary>
        /// Loads the assembly at the target path with all it's references, if possible.
        /// </summary>
        /// <param name="path">
        /// The path to the assembly to load.
        /// </param>
        /// <returns>
        /// The target assembly.
        /// </returns>
        public ILoadedAssemblyTarget LoadFileWithReferences(string path);

        #endregion
    }
}
