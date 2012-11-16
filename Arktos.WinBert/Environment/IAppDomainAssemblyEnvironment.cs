namespace Arktos.WinBert.Environment
{
    using System;

    /// <summary>
    /// Defines behavior for implementations that load assemblies into a built-in application domain.
    /// </summary>
    public interface IAppDomainAssemblyEnvironment : IAssemblyLoader, IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the contained application domain.
        /// </summary>
        AppDomain Domain { get; }

        #endregion
    }
}
