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
        /// Loads the target assembly with the indicated load method.
        /// </summary>
        /// <param name="loadMethod">
        /// The load method to use. Defaults to LoadMethod.Load.
        /// </param>
        /// <param name="assemblyPath">
        /// The path to the assembly to load.
        /// </param>
        /// <param name="pdbPath">
        /// The path to the PDB file. Defaults to null.
        /// </param>
        /// <returns>
        /// The target assembly.
        /// </returns>
        Assembly LoadAssembly(LoadMethod loadMethod, string assemblyPath, string pdbPath = null);

        #endregion
    }
}
