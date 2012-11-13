namespace Arktos.WinBert.Util
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.Cci;

    /// <summary>
    /// Handles loading assemblies into various contexts.
    /// </summary>
    public sealed class AssemblyResolver : IAssemblyResolver
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
        public AssemblyResolver()
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

        /// <inheritdoc />
        public Assembly LoadFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            return Assembly.LoadFile(path);
        }

        /// <inheritdoc />
        public Assembly LoadFrom(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            return Assembly.LoadFrom(path);
        }

        /// <inheritdoc />
        public Assembly LoadBits(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            byte[] bits = File.ReadAllBytes(path);
            return Assembly.Load(bits);
        }

        /// <inheritdoc />
        public Assembly LoadBits(string assemblyPath, string pdbPath)
        {
            if (string.IsNullOrEmpty(assemblyPath))
            {
                throw new ArgumentException("Assembly path cannot be null or empty!");
            }

            if (string.IsNullOrEmpty(pdbPath))
            {
                throw new ArgumentException("Pdb path cannot be null or empty!");
            }

            byte[] assemblyBits = File.ReadAllBytes(assemblyPath);
            byte[] pdbBits = File.ReadAllBytes(pdbPath);
            return Assembly.Load(assemblyBits, pdbBits);
        }

        /// <inheritdoc />
        public Assembly Resolve(object sender, ResolveEventArgs args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
