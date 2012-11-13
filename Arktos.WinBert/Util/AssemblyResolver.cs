namespace Arktos.WinBert.Util
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.Cci;

    /// <summary>
    /// Handles loading assemblies into various contexts.
    /// </summary>
    public class AssemblyResolver : MarshalByRefObject, IAssemblyResolver
    {
        #region Public Methods

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
