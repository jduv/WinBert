namespace Arktos.WinBert.Instrumentation
{
    using System;
    using System.Linq;
    using System.IO;
    using AppDomainToolkit;
    using Arktos.WinBert.Exceptions;
    using Arktos.WinBert.Extensions;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;

    /// <summary>
    /// Basic implementation of an instrumentation target.
    /// </summary>
    public sealed class InstrumentationTarget : IInstrumentationTarget
    {
        #region Fields & Constants

        private IMetadataHost host;
        private PdbReader pdbReader;
        private IAssembly mutableAssembly;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Prevents a default instance of the InstrumentationTarget class from being created.
        /// </summary>
        private InstrumentationTarget()
        {
            this.IsDisposed = false;
        }

        /// <summary>
        /// Finalizes an instance of the InstrumentationTarget class.
        /// </summary>
        ~InstrumentationTarget()
        {
            this.OnDispose(false);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public IAssemblyTarget Target { get; private set; }

        /// <inheritdoc />
        public IMetadataHost Host
        {
            get
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                return this.host;
            }

            private set
            {
                this.host = value;
            }
        }

        /// <inheritdoc />
        public PdbReader PdbReader
        {
            get
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                return this.pdbReader;
            }

            private set
            {
                this.pdbReader = value;
            }
        }

        /// <inheritdoc />
        public IAssembly MutableAssembly
        {
            get
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                return this.mutableAssembly;
            }

            private set
            {
                this.mutableAssembly = value;
            }
        }

        /// <inheritdoc />
        public bool IsDisposed { get; private set; }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Factory method for creating instrumentation targets. 
        /// </summary>
        /// <param name="target">
        /// The target assembly to prepare for instrumentation.
        /// </param>
        /// <returns>
        /// An instrumentation target.
        /// </returns>
        public static InstrumentationTarget Create(IAssemblyTarget target)
        {
            if(target == null)
            {
                throw new ArgumentNullException("target");
            }

            var host = new PeReader.DefaultHost();
            var mutableAssembly = GetMutableAssembly(target, host);
            var pdbReader = GetPdbReader(mutableAssembly, host);

            return new InstrumentationTarget()
            {
                Host = host,
                MutableAssembly = mutableAssembly,
                PdbReader = pdbReader,
                Target = target
            };
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.OnDispose(true);
        }

        /// <inheritdoc />
        public IAssemblyTarget Save()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException("Unable to save, object already disposed!");
            }

            var path = this.MutableAssembly.Location;
            var originalExt = Path.GetExtension(path);
            path = Path.ChangeExtension(path, ".instrumented" + originalExt);

            using (var file = File.Create(path))
            {
                if (this.pdbReader == null)
                {
                    PeWriter.WritePeToStream(this.MutableAssembly, host, file);
                }
                else
                {
                    var scopeProvider = this.PdbReader == null ? null : new ILGenerator.LocalScopeProvider(this.PdbReader);
                    using (var pdbWriter = new PdbWriter(path + ".pdb", this.PdbReader))
                    {
                        PeWriter.WritePeToStream(this.MutableAssembly, this.Host, file, this.PdbReader, scopeProvider, pdbWriter);
                    }
                }

                return AssemblyTarget.FromPath(new Uri(path), path, GetAssemblyName(this.MutableAssembly));
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a mutable copy of the target assembly.
        /// </summary>
        /// <param name="target">
        /// The assembly to retrieve the mutable copy for.
        /// </param>
        /// <param name="host">
        /// The host to use when loading the assembly.
        /// </param>
        /// <returns>
        /// A mutable copy of the target assembly.
        /// </returns>
        private static IAssembly GetMutableAssembly(IAssemblyTarget target, IMetadataHost host)
        {
            var copier = new MetadataDeepCopier(host);
            return copier.Copy(LoadModule(target, host));
        }

        /// <summary>
        /// Gets a PdbReader for the target module, inferring the proper paths where necessary.
        /// </summary>
        /// <param name="module">
        /// The module to fetch the PDB reader for.
        /// </param>
        /// <param name="host">
        /// The host to use when loading the reader.
        /// </param>
        /// <returns>
        /// A PdbReader for the target module's debug symbols or null if none exists.
        /// </returns>
        private static PdbReader GetPdbReader(IAssembly module, IMetadataHost host)
        {
            PdbReader reader = null;
            var secondaryPath = Path.ChangeExtension(module.Location, "pdb");
            var pdbFilePath = File.Exists(module.DebugInformationLocation) ? module.DebugInformationLocation :
                File.Exists(secondaryPath) ? secondaryPath : null;

            if(!string.IsNullOrEmpty(pdbFilePath))
            {
                var pdbStream = File.OpenRead(pdbFilePath);
                reader = new PdbReader(pdbStream, host);
            }
            return reader;
        }

        /// <summary>
        /// Loads the target module.
        /// </summary>
        /// <param name="target">
        /// The target to load.
        /// </param>
        /// <param name="host">
        /// The host to use when loading the module.
        /// </param>
        /// <returns>
        /// The target module.
        /// </returns>
        private static IAssembly LoadModule(IAssemblyTarget target, IMetadataHost host)
        {
            var location = string.IsNullOrEmpty(target.Location) ? target.CodeBase.LocalPath : target.Location;
            var module = host.LoadUnitFrom(location) as IAssembly;
            if (module == null || module is Dummy)
            {
                throw new FileNotFoundException("Unable to load assembly at location: " + target.Location);
            }

            return module;
        }

        /// <summary>
        /// Given a CCI metadata assembly, return the name in standard .NET Reflection format.
        /// </summary>
        /// <param name="assembly">
        /// The assembly whose name to build.
        /// </param>
        /// <returns>
        /// A string representing the full name of the target assembly.
        /// </returns>
        private static string GetAssemblyName(IAssembly assembly)
        {
            var culture = string.IsNullOrEmpty(assembly.Culture) ? "neutral" : assembly.Culture;
            return assembly.Name + ", Version=" + assembly.Version + 
                ", Culture=" + culture + 
                ", PublicKeyToken=" + Strings.PrettyPrintNullOrEmpty(Convert.ToBase64String(assembly.PublicKeyToken.ToArray()));
        }

        /// <summary>
        /// Called on dispose of this object.
        /// </summary>
        /// <param name="disposing">
        /// Was this method called from Dispose() or a finualizer?
        /// </param>
        private void OnDispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (disposing)
                {
                    // The host we're using is *likely* to be disposable, but check here anyhow.
                    var disposableHost = this.Host as System.IDisposable;
                    if (disposableHost != null)
                    {
                        disposableHost.Dispose();
                    }

                    if (this.PdbReader != null)
                    {
                        this.PdbReader.Dispose();
                    }

                    this.Host = null;
                    this.PdbReader = null;
                    this.MutableAssembly = null;
                }

                this.IsDisposed = true;
            }
        }

        #endregion
    }
}
