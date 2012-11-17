namespace Arktos.WinBert.Environment
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Loads assemblies into the contained application domain.
    /// </summary>
    public class AssemblyEnvironment : IAssemblyEnvironment
    {
        #region Fields & Constants

        private readonly AppDomain domain;
        private readonly Remote<RemotableAssemblyLoader> loaderProxy;
        private readonly Guid domainName;

        #endregion

        #region Constructrs & Destructors

        /// <summary>
        /// Initializes a new instance of the AppDomainAssemblyLoader class.
        /// </summary>
        public AssemblyEnvironment()
        {
            this.domainName = Guid.NewGuid();

            // Create the new domain.
            this.domain = AppDomain.CreateDomain(
                this.domainName.ToString(),
                null,
                AppDomain.CurrentDomain.SetupInformation);

            // Create a remote for an assembly loader.
            this.loaderProxy = Remote<RemotableAssemblyLoader>.Create(this.domain);
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
        public ILoadedAssemblyTarget LoadFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            if (!File.Exists(path))
            {
                throw new ArgumentException("Path must be an existing file!");
            }

            var assembly = this.loaderProxy.RemoteObject.LoadFile(path);
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This method will load the target assembly into the application domain wrapped by an instance
        /// of this class instead of the current one.
        /// </remarks>
        public ILoadedAssemblyTarget LoadFrom(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            if (!File.Exists(path))
            {
                throw new ArgumentException("Path must be an existing file!");
            }

            var assembly = this.loaderProxy.RemoteObject.LoadFrom(path);
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This method will load the target assembly into the application domain wrapped by an instance
        /// of this class instead of the current one.
        /// </remarks>
        public ILoadedAssemblyTarget LoadBits(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            if (!File.Exists(path))
            {
                throw new ArgumentException("Path must be an existing file!");
            }

            var assembly = this.loaderProxy.RemoteObject.LoadBits(path);
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This method will load the target assembly into the application domain wrapped by an instance
        /// of this class instead of the current one.
        /// </remarks>
        public ILoadedAssemblyTarget LoadBits(string assemblyPath, string pdbPath)
        {
            if (string.IsNullOrEmpty(assemblyPath))
            {
                throw new ArgumentException("Assembly path cannot be null or empty!");
            }

            if (!File.Exists(assemblyPath))
            {
                throw new ArgumentException("Assembly path must be an existing file!");
            }

            if (string.IsNullOrEmpty(pdbPath))
            {
                throw new ArgumentException("Pdb path cannot be null or empty!");
            }

            if (!File.Exists(pdbPath))
            {
                throw new ArgumentException("Pdb path must be an existing file!");
            }

            var assembly = this.loaderProxy.RemoteObject.LoadBits(assemblyPath, pdbPath);
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This method will load the target assembly into the application domain wrapped by an instance
        /// of this class instead of the current one.
        /// </remarks>
        public ILoadedAssemblyTarget LoadFileWithReferences(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            if (!File.Exists(path))
            {
                throw new ArgumentException("Path must be an existing file!");
            }

            throw new NotImplementedException();
        }

        #endregion
    }
}
