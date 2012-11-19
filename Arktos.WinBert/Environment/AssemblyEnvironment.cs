namespace Arktos.WinBert.Environment
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Loads assemblies into the contained application domain.
    /// BMK: Clean this up.
    /// </summary>
    public class AssemblyEnvironment : IAssemblyEnvironment
    {
        #region Fields & Constants

        private readonly AppDomain domain;
        private readonly Remote<RemotableAssemblyLoader> loaderProxy;
        private readonly Guid domainName;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the AppDomainAssemblyLoader class.
        /// </summary>
        public AssemblyEnvironment()
        {
            this.domainName = Guid.NewGuid();
            this.Resolver = new AssemblyResolver();

            // All app domains must be loaded with the WinBert plugin context.
            var winbertDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var setupInfo = new AppDomainSetup()
            {
                ApplicationName = "WinBert-Temp-Domain-" + this.domainName,
                ApplicationBase = winbertDir,
                PrivateBinPath = winbertDir
            };

            // Add the root directory for this assembly to the resolver.
            this.Resolver.AddProbePath(winbertDir);

            // Create the new domain.
            this.domain = AppDomain.CreateDomain(
                this.domainName.ToString(),
                null,
                setupInfo);

            this.domain.AssemblyResolve += this.Resolver.Resolve;
            //AppDomain.CurrentDomain.AssemblyResolve += this.Resolver.Resolve;

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

        /// <inheritdoc />
        public IAssemblyResolver Resolver { get; private set; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public void Dispose()
        {
            if (this.domain != null && !this.domain.IsDefaultAppDomain())
            {
                AppDomain.Unload(this.domain);
            }
        }

        /// <inheritdoc />
        public ILoadedAssemblyTarget LoadTarget(IAssemblyTarget target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (string.IsNullOrEmpty(target.Location))
            {
                throw new ArgumentException("Target location cannot be null or empty.");
            }

            if (!File.Exists(target.Location))
            {
                throw new FileNotFoundException("Target location must be an existing file.");
            }

            return this.LoadFile(target.Location);
        }

        /// <inheritdoc/>
        public ILoadedAssemblyTarget LoadFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Path must be an existing file!");
            }

            var assembly = this.loaderProxy.RemoteObject.LoadFile(path);
            if (assembly != null)
            {
                return new LoadedAssemblyTarget(assembly, this);
            }
            else
            {
                // BMK Throw exception here.
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public ILoadedAssemblyTarget LoadFrom(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Path must be an existing file!");
            }

            var assembly = this.loaderProxy.RemoteObject.LoadFrom(path);
            if (assembly != null)
            {
                return new LoadedAssemblyTarget(assembly, this);
            }
            else
            {
                // BMK Throw exception here.
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public ILoadedAssemblyTarget LoadBits(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Path must be an existing file!");
            }

            var assembly = this.loaderProxy.RemoteObject.LoadBits(path);
            if (assembly != null)
            {
                return new LoadedAssemblyTarget(assembly, this);
            }
            else
            {
                // BMK Throw exception here.
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public ILoadedAssemblyTarget LoadBits(string assemblyPath, string pdbPath)
        {
            if (string.IsNullOrEmpty(assemblyPath))
            {
                throw new ArgumentException("Assembly path cannot be null or empty!");
            }

            if (!File.Exists(assemblyPath))
            {
                throw new FileNotFoundException("Assembly path must be an existing file!");
            }

            if (string.IsNullOrEmpty(pdbPath))
            {
                throw new ArgumentException("Pdb path cannot be null or empty!");
            }

            if (!File.Exists(pdbPath))
            {
                throw new FileNotFoundException("Pdb path must be an existing file!");
            }

            var assembly = this.loaderProxy.RemoteObject.LoadBits(assemblyPath, pdbPath);
            if (assembly != null)
            {
                return new LoadedAssemblyTarget(assembly, this);
            }
            else
            {
                // BMK Throw exception here.
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Inner Classes

        /// <summary>
        /// Represents an assembly loaded into an environment.
        /// </summary>
        private class LoadedAssemblyTarget : ILoadedAssemblyTarget
        {
            #region Constructors & Destructors

            /// <summary>
            /// Initializes a new instance of the LoadedAssemblyTarget class.
            /// </summary>
            /// <param name="loadedAssembly">
            /// The loaded assembly.
            /// </param>
            /// <param name="environment">
            /// The environment where the assembly was loaded.
            /// </param>
            public LoadedAssemblyTarget(Assembly loadedAssembly, IAssemblyEnvironment environment)
            {
                if (Assembly == null)
                {
                    throw new ArgumentNullException("loadedAssembly");
                }

                if (environment != null)
                {
                    throw new ArgumentNullException("environment");
                }

                this.Assembly = loadedAssembly;
                this.Environment = environment;
            }

            #endregion

            #region Properties

            /// <inheritdoc />
            public Assembly Assembly { get; private set; }

            /// <inheritdoc />
            public IAssemblyEnvironment Environment { get; private set; }

            /// <inheritdoc />
            public string Location
            {
                get
                {
                    return this.Assembly.Location;
                }
            }

            #endregion
        }

        #endregion
    }
}
