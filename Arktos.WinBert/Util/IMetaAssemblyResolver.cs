namespace Arktos.WinBert.Util
{
    using Microsoft.Cci;
    using System.Reflection;

    /// <summary>
    /// Defines behavior for an assembly resolver that can handle metadata.
    /// </summary>
    public interface IMetaAssemblyResolver
    {
        #region Methods

        /// <summary>
        /// Loads the metadata for the assembly at the target path.
        /// </summary>
        /// <param name="path">
        /// The path to the assembly to load.
        /// </param>
        /// <param name="host">
        /// The host used to load the assembly.
        /// </param>
        /// <returns>
        /// The metadata for the assembly.
        /// </returns>
        IAssembly LoadMeta(string path);

        /// <summary>
        /// Loads the target assembly into the current application domain given 
        /// it's metadata.
        /// </summary>
        /// <param name="meta">
        /// The metadata of the assembly to load.
        /// </param>
        /// <returns>
        /// The loaded assembly.
        /// </returns>
        Assembly FromMeta(IAssembly meta);

        #endregion
    }
}
