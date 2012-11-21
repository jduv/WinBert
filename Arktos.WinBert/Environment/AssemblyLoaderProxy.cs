namespace Arktos.WinBert.Environment
{
    using System;

    /// <summary>
    /// This class exists to prevent DLL hell. Assemblies must be loaded into specific application domains
    /// without crossing those boundaries. We cannot simply remote an AssemblyLoader into a remote 
    /// domain and load assemblies to use in the current domain. Instead, we introduct a tiny, serializable
    /// implementation of the AssemblyTarget class that handles comunication between the foreign app
    /// domain and the default one. This class is simply a wrapper around an assembly loader that translates
    /// Assembly to AssemblyTarget instances before shipping them back to the parent domain.
    /// </summary>
    public class AssemblyLoaderProxy : MarshalByRefObject
    {
        #region Fields & Constants

        private readonly IAssemblyLoader loader;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the RemotableAssemblyLoader class.
        /// </summary>
        public AssemblyLoaderProxy()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RemotableAssemblyLoader class.
        /// </summary>
        /// <param name="loader">
        /// The AssemblyLoader to use when importing assemblies.
        /// </param>
        public AssemblyLoaderProxy(IAssemblyLoader loader)
        {
            this.loader = loader == null ? new AssemblyLoader() : loader;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads an assembly into the current application domain and returns a serializable instance of the AssemblyTarget
        /// class for the parent application domain's consumption.
        /// </summary>
        /// <param name="loadMethod"></param>
        /// <param name="assemblyPath"></param>
        /// <param name="pdbPath"></param>
        /// <returns></returns>
        public AssemblyTarget LoadAssembly(LoadMethod loadMethod, string assemblyPath, string pdbPath = null)
        {
            return AssemblyTarget.Create(this.loader.LoadAssembly(loadMethod, assemblyPath, pdbPath));
        }

        #endregion
    }
}
