namespace Arktos.WinBert.Environment
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Used to determine which load context assemblies should be loaded into by the resolver.
    /// </summary>
    public enum LoadMethod
    {
        /// <summary>
        /// The default load context. Use this if your application has it's bits located in the GAC or
        /// on the main application path of the AppDomain.CurrentDomain. This should be rare unless you
        /// like putting Assemblies in the GAC.
        /// </summary>
        Load,

        /// <summary>
        /// Loads the assembly into the LoadFrom context, which enables the assembly and all it's references to be discovered
        /// and loaded into the target application domain. Despite it's penchant for DLL hell, this is probably the way to go by
        /// default as long as you make sure to pass the base directory of the application to an AssemblyResolver instance such
        /// that references can be properly resolved. This also allows for multiple assemblies of the same name to be loaded while
        /// maintaining separate file names. This is the recommended way to go.
        /// </summary>
        LoadFrom,

        /// <summary>
        /// Loads an assembly into memory using the raw file name. This loads the assembly anonymously, so it won't have
        /// a load context. Use this if you want the bits loaded, but make sure to pass the directory where this file lives to an 
        /// AssemblyResolver instance so you can find it again. This is similar to LoadFrom except you don't get the free 
        /// lookups for already existing assembly names via fusion. Use this for more control over assembly file loads.
        /// </summary>
        LoadFile,

        /// <summary>
        /// Loads the bits of the target assembly into memory using the raw file name. This is, in essence, a dynamic assembly
        /// for all the CLR cares. You won't ever be able to find this with an assembly resolver, so don't use this unless you look
        /// for it by name. Be careful with this one.
        /// </summary>
        LoadBits
    }

    /// <summary>
    /// Handles resolving assemblies in application domains. This class is helpful when attempting to load a
    /// particular assembly into an application domain and the assembly you're looking for doesn't exist in the
    /// main application bin path. This 'feature' of the .NET framework makes assembly loading very, very
    /// irritating, but this little helper class should alleviate much of the pains here. Note that it extends 
    /// MarshalByRefObject, so it can be remoted into another application domain.
    /// </summary>
    public class AssemblyResolver : MarshalByRefObject, IAssemblyResolver
    {
        #region Fields & Constants

        private readonly HashSet<string> probePaths;
        private readonly IAssemblyLoader loader;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the AssemblyResolver class. A default instance of this class will resolve
        /// assemblies into the LoadFrom context.
        /// </summary>
        /// <param name="loader">
        /// The loader to use when loading assemblies. Default is null, which will create and use an instance
        /// of the RemotableAssemblyLoader class.
        /// </param>
        /// <param name="loadMethod">
        /// The load method to use when loading assemblies. Defaults to LoadMethod.LoadFrom.
        /// </param>
        public AssemblyResolver(
            IAssemblyLoader loader = null, 
            LoadMethod loadMethod = LoadMethod.LoadFrom)
        {
            this.probePaths = new HashSet<string>();
            this.loader = loader == null ? new AssemblyLoader() : loader;
            this.LoadMethod = loadMethod;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public LoadMethod LoadMethod { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Convenience cast.
        /// </summary>
        /// <param name="resolver">
        /// The resolver to cast.
        /// </param>
        /// <returns>
        /// A ResolveEventHandler.
        /// </returns>
        public static implicit operator ResolveEventHandler(AssemblyResolver resolver)
        {
            ResolveEventHandler handler = null;
            if (resolver != null)
            {
                handler = resolver.Resolve;
            }

            return handler;
        }

        /// <inheritdoc />
        public void AddProbePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            var dir = new DirectoryInfo(path);
            if (!this.probePaths.Contains(dir.FullName))
            {
                this.probePaths.Add(dir.FullName);
            }
        }

        /// <inheritdoc />
        public Assembly Resolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);
            foreach (var path in this.probePaths)
            {
                var dllPath = Path.Combine(path, string.Format("{0}.dll", name.Name));
                if (File.Exists(dllPath))
                {
                    return this.loader.Load(dllPath, this.LoadMethod);
                }

                var exePath = Path.ChangeExtension(dllPath, "exe");
                if (File.Exists(exePath))
                {
                    return this.loader.Load(exePath, this.LoadMethod);
                }
            }

            // Not found.
            return null;
        }

        #endregion
    }
}
