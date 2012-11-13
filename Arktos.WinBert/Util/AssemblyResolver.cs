namespace Arktos.WinBert.Util
{
    using System;
    using System.Reflection;
    using Microsoft.Cci;

    /// <summary>
    /// Handles loading assemblies into various contexts.
    /// </summary>
    public sealed class AssemblyResolver : IAssemblyResolver
    {
        #region Public Methods

        /// <inheritdoc />
        public IAssembly LoadMeta(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Assembly FromMeta(IAssembly meta)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Assembly LoadFile(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Assembly LoadFrom(string path)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Assembly LoadBits(string assemblyPath)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Assembly LoadBits(string assemblyPath, string pdbPath)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Assembly Resolve(object sender, ResolveEventArgs args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
