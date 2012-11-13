namespace Arktos.WinBert.Environment
{
    using System;
    using Microsoft.Cci;

    /// <summary>
    /// Defines behavior for implementations that can load files into the contained CCI metdata host.
    /// </summary>
    public interface IMetadataHostAssemblyEnvironment : IDisposable
    {
        #region Methods

        /// <summary>
        /// Loads the assembly at the target path into the host and returns metadata about it.
        /// </summary>
        /// <param name="path">
        /// The path to the assembly to load.
        /// </param>
        /// <returns>
        /// Assembly metadata.
        /// </returns>
        IAssembly LoadFile(string path);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the host where all metadata will be loaded into.
        /// </summary>
        IMetadataHost Host { get; }

        #endregion
    }
}
