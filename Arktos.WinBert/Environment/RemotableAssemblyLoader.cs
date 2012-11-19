namespace Arktos.WinBert.Environment
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// This inner class exists to pull assemblies into whatever application domain it's loaded into.\
    /// BMK: Clean this class up.
    /// </summary>
    public class RemotableAssemblyLoader : MarshalByRefObject, IAssemblyLoader
    {
        #region Public Methods
        
        /// <inheritdoc /> 
        public Assembly Load(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            if (!File.Exists(path))
            {
                throw new ArgumentException("Path must be an existing file!");
            }

            return Assembly.Load(path);
        }

        /// <inheritdoc/>
        public Assembly LoadFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            if (!File.Exists(path))
            {
                throw new ArgumentException("Path must be an existing file!");
            }

            return Assembly.LoadFile(path);
        }

        /// <inheritdoc/>
        public Assembly LoadFrom(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            if (!File.Exists(path))
            {
                throw new ArgumentException("Path must be an existing file!");
            }

            return Assembly.LoadFrom(path);
        }

        /// <inheritdoc/>
        public Assembly LoadBits(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            if (!File.Exists(path))
            {
                throw new ArgumentException("Path must be an existing file!");
            }

            byte[] bits = File.ReadAllBytes(path);
            return Assembly.Load(bits);
        }

        /// <inheritdoc/>
        public Assembly LoadBits(string assemblyPath, string pdbPath)
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

            byte[] assemblyBits = File.ReadAllBytes(assemblyPath);
            byte[] pdbBits = File.ReadAllBytes(pdbPath);
            return Assembly.Load(assemblyBits, pdbBits);
        }

        #endregion
    }
}
