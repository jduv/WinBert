using System;
using Microsoft.Cci;
using System.Reflection;

namespace Arktos.WinBert.Util
{
    /// <summary>
    /// Handles loading assembiles via a host.
    /// </summary>
    public class MetaAssemblyResolver : AssemblyResolver, IMetaAssemblyResolver
    {
        #region Fields & Constants

        private readonly IMetadataHost host;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Static constructor. Creates the metadata host used to load and resolve assemblies.
        /// The Host object will be unloaded when the app domain dies, so there shouldn't be
        /// a need to manage the lifecycle of it.
        /// </summary>
        public MetaAssemblyResolver()
        {
            this.host = new PeReader.DefaultHost();
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public IAssembly LoadMeta(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            return (IAssembly)this.host.LoadUnitFrom(path);
        }

        /// <inheritdoc />
        public Assembly FromMeta(IAssembly meta)
        {
            if (meta == null)
            {
                throw new ArgumentNullException("meta");
            }

            if (string.IsNullOrEmpty(meta.Location))
            {
                throw new ArgumentException("Unable to locate assembly!");
            }

            return this.LoadFile(meta.Location);
        }

        #endregion
    }
}
