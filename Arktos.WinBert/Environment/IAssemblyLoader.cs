namespace Arktos.WinBert.Environment
{
    using System.Reflection;

    /// <summary>
    /// Defines behavior for a class that acts exactly the same as the static methods on
    /// the Assembly class.
    /// </summary>
    public interface IAssemblyLoader
    {
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
        Assembly LoadFile(string path);

        /// <summary>
        /// This call should be the same as calling Assembly.LoadFrom().
        /// </summary>
        /// <param name="path">
        /// The path to the assembly to load.
        /// </param>
        /// <returns>
        /// The target assembly.
        /// </returns>
        Assembly LoadFrom(string path);

        /// <summary>
        /// Loads the bits of the assembly at the target path into the current application domain.
        /// </summary>
        /// <param name="path">
        /// The path to the assembly to load.
        /// </param>
        /// <returns>
        /// The target assembly.
        /// </returns>
        Assembly LoadBits(string path);

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
        Assembly LoadBits(string assemblyPath, string pdbPath);

        /// <summary>
        /// Loads the assembly at the target path along with all referenced assemblies into the
        /// contained application domain.
        /// </summary>
        /// <param name="path">
        /// The path to the assembly to load.
        /// </param>
        /// <returns>
        /// The assembly that was loaded.
        /// </returns>
        Assembly LoadFileWithReferences(string path);

        #endregion
    }
}
