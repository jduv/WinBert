namespace Arktos.WinBert.Environment
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Loads assemblies into the contained application domain.
    /// </summary>
    public class AppDomainAssemblyEnvironment : IAppDomainAssemblyEnvironment
    {
        #region Fields & Constants

        private readonly AppDomain domain;
        private readonly AssemblyLoaderProxy proxy;
        private readonly Guid domainName;

        #endregion

        #region Constructrs & Destructors

        /// <summary>
        /// Initializes a new instance of the AppDomainAssemblyLoader class.
        /// </summary>
        public AppDomainAssemblyEnvironment()
        {
            this.domainName = Guid.NewGuid();

            // Create the new domain.
            this.domain = AppDomain.CreateDomain(
                this.domainName.ToString(),
                null,
                AppDomain.CurrentDomain.SetupInformation);

            // Create the proxy.
            var type = typeof(AssemblyLoaderProxy);
            this.proxy = (AssemblyLoaderProxy)this.domain.CreateInstanceAndUnwrap(
                type.Assembly.FullName,
                type.FullName);
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public AppDomain Domain
        {
            get
            {
                return this.domain;
            }
        }

        /// <summary>
        /// Gets the name assigned to the contained app domain.
        /// </summary>
        public Guid DomainName
        {
            get
            {
                return this.domainName;
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        /// <remarks>
        /// Dispose will unload the application domain managed by the loader if it' *not* the default app domain.
        /// This should always be the case, since the ctor creates the application domain that we use anyway--but 
        /// better safe than crashed application.
        /// </remarks>
        public void Dispose()
        {
            if (this.domain != null && !this.domain.IsDefaultAppDomain())
            {
                AppDomain.Unload(this.domain);
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This method will load the target assembly into the application domain wrapped by an instance
        /// of this class instead of the current one.
        /// </remarks>
        public Assembly LoadFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            return this.proxy.LoadFile(path);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This method will load the target assembly into the application domain wrapped by an instance
        /// of this class instead of the current one.
        /// </remarks>
        public Assembly LoadFileWithReferences(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This method will load the target assembly into the application domain wrapped by an instance
        /// of this class instead of the current one.
        /// </remarks>
        public Assembly LoadFrom(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            return this.proxy.LoadFrom(path);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This method will load the target assembly into the application domain wrapped by an instance
        /// of this class instead of the current one.
        /// </remarks>
        public Assembly LoadBits(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            return this.proxy.LoadBits(path);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This method will load the target assembly into the application domain wrapped by an instance
        /// of this class instead of the current one.
        /// </remarks>
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

            return this.proxy.LoadBits(assemblyPath, pdbPath);
        }

        #endregion

        #region Private Inner Class

        /// <summary>
        /// This inner class exists to pull assemblies into whatever application domain it's loaded into.
        /// </summary>
        private class AssemblyLoaderProxy : MarshalByRefObject, IAssemblyLoader
        {
            /// <inheritdoc/>
            public Assembly LoadFile(string path)
            {
                return Assembly.LoadFile(path);
            }

            /// <inheritdoc/>
            public Assembly LoadFrom(string path)
            {
                return Assembly.LoadFrom(path);
            }

            /// <inheritdoc/>
            public Assembly LoadBits(string path)
            {
                byte[] bits = File.ReadAllBytes(path);
                return Assembly.Load(bits);
            }

            /// <inheritdoc/>
            public Assembly LoadBits(string assemblyPath, string pdbPath)
            {
                byte[] assemblyBits = File.ReadAllBytes(assemblyPath);
                byte[] pdbBits = File.ReadAllBytes(pdbPath);
                return Assembly.Load(assemblyBits, pdbBits);
            }
        }

        #endregion
    }
}
