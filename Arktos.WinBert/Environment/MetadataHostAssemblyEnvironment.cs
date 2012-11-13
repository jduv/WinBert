namespace Arktos.WinBert.Environment
{
    using System;
    using Microsoft.Cci;

    /// <summary>
    /// Handles pulling assemblies into the contained IMetadataHost instance.
    /// </summary>
    public class MetadataHostAssemblyEnvironment : IMetadataHostAssemblyEnvironment
    {
        #region Fields & Constants

        private readonly IMetadataHost host;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the MetadataHostAssemblyEnvironment class.
        /// </summary>
        public MetadataHostAssemblyEnvironment() : this(new PeReader.DefaultHost())
        {
        }

        /// <summary>
        /// Initializes a new instance of the MetadataHostAssemblyEnvironment class.
        /// </summary>
        /// <param name="host">The host to initialize with.</param>
        public MetadataHostAssemblyEnvironment(IMetadataHost host)
        {
            this.host = host;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public IMetadataHost Host
        {
            get 
            {
                return this.host;
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public void Dispose()
        {
            // Attempt to dispose the host.
            var disposableHost = this.host as IDisposable;
            if (disposableHost != null)
            {
                disposableHost.Dispose();
            }
        }

        /// <inheritdoc />
        public IAssembly LoadFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            return (IAssembly)this.host.LoadUnitFrom(path);
        }

        #endregion
    }
}
