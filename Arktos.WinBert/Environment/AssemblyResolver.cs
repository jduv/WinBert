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

        private readonly IDictionary<string, DirectoryInfo> probePaths;
        private readonly IAssemblyLoader loader;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the AssemblyResolver class. The default load context for
        /// a new instance is the LoadFrom context.
        /// </summary>
        public AssemblyResolver()
            : this(null, LoadMethod.LoadFrom)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AssemblyResolver class.
        /// </summary>
        /// <param name="loader">
        /// The loader to use when loading assemblies.
        /// </param>
        /// <param name="context">
        /// The context to load assemblies into.
        /// </param>
        public AssemblyResolver(IAssemblyLoader loader, LoadMethod context)
        {
            this.probePaths = new Dictionary<string, DirectoryInfo>();
            this.loader = loader;
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
                path = Directory.GetCurrentDirectory();
            }

            var info = new DirectoryInfo(path);
            if (!this.probePaths.ContainsKey(info.FullName))
            {
                this.probePaths.Add(info.FullName, info);
            }
        }

        /// <inheritdoc />
        public Assembly Resolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);
            foreach (var info in this.probePaths.Values)
            {
                var dllPath = Path.Combine(info.FullName, string.Format("{0}.dll", name.Name));
                if (File.Exists(dllPath))
                {
                    return this.LoadAssembly(dllPath);
                }

                var exePath = Path.ChangeExtension(dllPath, "exe");
                if (File.Exists(exePath))
                {
                    return this.LoadAssembly(exePath);
                }
            }

            // Not found.
            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads an assembly given a valid path. This method uses the LoadMethod defined on this
        /// AssemblyResolver instance to determine how it loadds the target assembly.
        /// </summary>
        /// <param name="path">
        /// The path to the assembly to load. It is assumed to exist.
        /// </param>
        /// <returns>
        /// The loaded assembly.
        /// </returns>
        private Assembly LoadAssembly(string path)
        {
            switch (this.LoadMethod)
            {
                case LoadMethod.Load:
                    return this.loader == null ? Assembly.Load(path) : this.loader.Load(path);
                case LoadMethod.LoadFrom:
                    return this.loader == null ? Assembly.LoadFrom(path) : this.loader.LoadFrom(path);
                case LoadMethod.LoadFile:
                    return this.loader == null ? Assembly.LoadFile(path) : this.loader.LoadFile(path);
                case LoadMethod.LoadBits:
                    Assembly assembly = null;
                    var pdbPath = Path.ChangeExtension(path, "pdb");
                    if (File.Exists(pdbPath))
                    {
                        assembly = this.loader == null ? Assembly.Load(File.ReadAllBytes(path), File.ReadAllBytes(pdbPath)) : this.loader.LoadBits(path, pdbPath);
                    }
                    else
                    {
                        assembly = this.loader == null ? Assembly.Load(File.ReadAllBytes(path)) : this.loader.LoadBits(path);
                    }

                    return assembly;
                default:
                    throw new NotSupportedException("The target load method isn't supported!");
            }
        }

        #endregion
    }
}
