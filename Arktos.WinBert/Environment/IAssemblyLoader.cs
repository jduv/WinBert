namespace Arktos.WinBert.Environment
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Defines behavior for an instance class that should act similar to the static methods on the
    /// Assembly class. The reason for this is to support hot-swappable loading implemenations and
    /// facilitate better testing of encironments by stubbing out how each load method executes.
    /// </summary>
    public interface IAssemblyLoader
    {
        #region Methods

        /// <summary>
        /// Loads the target assembly with the indicated load method. This method should default to the same as
        /// calling Assembly.Load(). If LoadMethod.LoadBits is passed to the method then it should attempt to also load
        /// the PDB file along side the target assembly if it exists. If this contract isn't kept then there's a possibility of
        /// file image exceptions being thrown as the load context needs the debug symbols in order to adequately load
        /// the assembly.
        /// </summary>
        /// <param name="assemblyPath">
        /// The path to the assembly to load.
        /// </param>
        /// <param name="loadMethod">
        /// The load method to use. Defaults to LoadMethod.Load.
        /// </param>
        /// <returns>
        /// The target assembly.
        /// </returns>
        Assembly Load(string assemblyPath, LoadMethod loadMethod = LoadMethod.Load);

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

        #endregion
    }
}
