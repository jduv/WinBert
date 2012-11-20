namespace Arktos.WinBert.Environment
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// This class will load assemblies into whatever application domain it's loaded into.
    /// </summary>
    public class AssemblyLoader : MarshalByRefObject, IAssemblyLoader
    {
        #region Public Methods

        /// <inheritdoc />
        public Assembly Load(string assemblyPath, LoadMethod method)
        {
            if (string.IsNullOrEmpty(assemblyPath))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            if (!File.Exists(assemblyPath))
            {
                throw new FileNotFoundException("Path must be an existing file!");
            }

            switch (method)
            {
                case LoadMethod.Load:
                    return this.Load(assemblyPath);
                case LoadMethod.LoadFrom:
                    return this.LoadFrom(assemblyPath);
                case LoadMethod.LoadFile:
                    return this.LoadFile(assemblyPath);
                case LoadMethod.LoadBits:

                    // Attempt to load the PDB bits along with the assembly to avoid image exceptions.
                    Assembly assembly = null;
                    var pdbPath = Path.ChangeExtension(assemblyPath, "pdb");
                    if (File.Exists(pdbPath))
                    {
                        assembly = this.LoadBits(assemblyPath, pdbPath);
                    }
                    else
                    {
                        assembly = this.LoadBits(assemblyPath);
                    }

                    return assembly;
                default:
                    throw new NotSupportedException("The target load method isn't supported!");
            }
        }

        /// <inheritdoc /> 
        public Assembly Load(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Path must be an existing file!");
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
                throw new FileNotFoundException("Path must be an existing file!");
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
                throw new FileNotFoundException("Path must be an existing file!");
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
                throw new FileNotFoundException("Path must be an existing file!");
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

            byte[] assemblyBits = File.ReadAllBytes(assemblyPath);
            byte[] pdbBits = File.ReadAllBytes(pdbPath);
            return Assembly.Load(assemblyBits, pdbBits);
        }

        #endregion
    }
}
