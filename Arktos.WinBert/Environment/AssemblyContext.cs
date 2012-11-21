namespace Arktos.WinBert.Environment
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Loads assemblies into the contained application domain.
    /// BMK: Clean this up.
    /// </summary>
    public class AssemblyContext : IAssemblyContext
    {
        #region Fields & Constants

        private readonly AppDomain domain;
        private readonly Remote<AssemblyLoaderProxy> loaderProxy;
        private readonly Guid domainName;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the AppDomainAssemblyLoader class. The assembly environment will create
        /// a new application domain with the location of the currently executing assembly as the application base. It
        /// will also add that root directory to the assembly resolver's path in order to properly load a remotable
        /// AssemblyLoader object into context. From here, add whatever assembly probe paths you wish in order to
        /// resolve remote proxies, or extend this class if you desire more specific behavior.
        /// </summary>
        public AssemblyContext()
        {
            this.domainName = Guid.NewGuid();
            this.Resolver  = new AssemblyResolver();

            var rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var setupInfo = new AppDomainSetup()
            {
                ApplicationName = "WinBert-Temp-Domain-" + this.domainName,
                ApplicationBase = rootDir,
                PrivateBinPath = rootDir
            };

            // Add the root directory for this assembly to the resolver.
            this.Resolver.AddProbePath(rootDir);

            // Create the new domain.
            this.domain = AppDomain.CreateDomain(
                this.domainName.ToString(),
                null,
                setupInfo);

            this.domain.AssemblyResolve += this.Resolver.Resolve;
            AppDomain.CurrentDomain.AssemblyResolve += this.Resolver.Resolve;


            // Create a remote for an assembly loader.
            this.loaderProxy = Remote<AssemblyLoaderProxy>.CreateProxy(this.domain);
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
        /// Gets a unique ID assigned to the environment. Useful for dictionary keys.
        /// </summary>
        public Guid UniqueId
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
        public AssemblyTarget LoadTarget(LoadMethod loadMethod, AssemblyTarget target)
        {
            return this.LoadAssembly(loadMethod, target.Location);
        }

        /// <inheritdoc/>
        public AssemblyTarget LoadAssembly(LoadMethod loadMethod, string assemblyPath, string pdbPath = null)
        {
            return this.loaderProxy.RemoteObject.LoadAssembly(loadMethod, assemblyPath, pdbPath);
        }

        #endregion
    }
}
