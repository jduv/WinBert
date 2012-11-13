namespace Arktos.WinBert.Util
{
    using System;
    using System.Reflection;
    using Microsoft.Cci;

    /// <summary>
    /// Implementations of this interface should be able to resolve and load assemblies into
    /// the current application domain.
    /// </summary>
    public interface IAssemblyResolver
    {
        #region Interface Methods

        /// <summary>
        /// Loads the metadata for the assembly at the target path.
        /// </summary>
        /// <param name="path">
        /// The path to the assembly to load.
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

        /// <summary>
        /// This should be same as calling Assembly.LoadFile.
        /// </summary>
        /// <param name="path">
        /// The path to the assembly to load.
        /// </param>
        /// <returns>
        /// The loaded assembly.
        /// </returns>
        Assembly LoadFile(string path);

        /// <summary>
        /// This should be same as calling Assembly.LoadFrom.
        /// </summary>
        /// <param name="path">
        /// The path to the assembly to load.
        /// </param>
        /// <returns>
        /// The loaded assembly.
        /// </returns>
        Assembly LoadFrom(string path);

        /// <summary>
        /// Loads the bits of the target assembly and then imports it into the
        /// current application domain. Loading an assembly this way will not lock
        /// the file--but it will also result in the location property being non-existant.
        /// </summary>
        /// <param name="assemblyPath">
        /// The path to the assembly to load.
        /// </param>
        /// <returns>
        /// The loaded assembly.
        /// </returns>
        Assembly LoadBits(string assemblyPath);

        /// <summary>
        /// Loads the bits of the target assembly and then imports it into the
        /// current application domain. Loading an assembly this way will not lock
        /// the file--but it will also result in the location property being non-existant.
        /// </summary>
        /// <param name="assemblyPath">
        /// The path to the assembly to load.
        /// </param>
        /// <param name="pdbPath">
        /// The path to the debug information for the assembly to load.
        /// </param>
        /// <returns>
        /// The loaded assembly.
        /// </returns>
        Assembly LoadBits(string assemblyPath, string pdbPath);

        /// <summary>
        /// Resolve event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="args">
        /// Event arguments.
        /// </param>
        /// <returns>
        /// The loaded assembly.
        /// </returns>
        Assembly Resolve(object sender, ResolveEventArgs args);

        #endregion
    }
}
