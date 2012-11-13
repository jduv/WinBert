namespace Arktos.WinBert.Environment
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Defines behavior for implementations that load assemblies into a built-in application domain.
    /// </summary>
    public interface IAppDomainAssemblyEnvironment : IAssemblyLoader, IDisposable
    {
        #region Methods

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

        #region Properties

        /// <summary>
        /// Gets the contained application domain.
        /// </summary>
        AppDomain Domain { get; }

        #endregion
    }
}
