namespace Arktos.WinBert.Instrumentation
{
    using AppDomainToolkit;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;

    /// <summary>
    /// Defines behavior for instrumentation targets.
    /// </summary>
    public interface IInstrumentationTarget : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the metadata host used for setting an environment to instrument the
        /// target binaries.
        /// </summary>
        IMetadataHost Host { get; }
        
        /// <summary>
        /// Gets an object used for handling debug information for the target binary.
        /// </summary>
        PdbReader PdbReader { get; }

        /// <summary>
        /// Gets the mutable assembly.
        /// </summary>
        Assembly MutableAssembly { get; }

        /// <summary>
        /// Gets the local scope provider.
        /// </summary>
        ILocalScopeProvider LocalScopeProvider { get; }

        /// <summary>
        /// Gets the assembly target.
        /// </summary>
        IAssemblyTarget Target { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Saves the target mutable module out to the file path.
        /// </summary>
        /// <returns>
        /// An AssemblyTarget pointing to the file location.
        /// </returns>
        IAssemblyTarget Save();

        #endregion
    }
}
