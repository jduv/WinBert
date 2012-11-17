namespace Arktos.WinBert.Environment
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// This inner class exists to pull assemblies into whatever application domain it's loaded into.
    /// </summary>
    private class RemotableAssemblyLoader : MarshalByRefObject, IAssemblyLoader
    {
        /// <inheritdoc/>
        public Assembly LoadFile(string path)
        {
            return Assembly.LoadFile(path);
        }

        /// <inheritdoc/>
        public Assembly LoadFrom(string path)
        {
            return Assembly.LoadFrom(path);
        }

        /// <inheritdoc/>
        public Assembly LoadBits(string path)
        {
            byte[] bits = File.ReadAllBytes(path);
            return Assembly.Load(bits);
        }

        /// <inheritdoc/>
        public Assembly LoadBits(string assemblyPath, string pdbPath)
        {
            byte[] assemblyBits = File.ReadAllBytes(assemblyPath);
            byte[] pdbBits = File.ReadAllBytes(pdbPath);
            return Assembly.Load(assemblyBits, pdbBits);
        }
    }
}
