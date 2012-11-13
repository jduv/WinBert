﻿namespace Arktos.WinBert.Testing
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Arktos.WinBert.Exceptions;
    using Arktos.WinBert.Util;
    using Microsoft.Cci;

    /// <summary>
    /// Compiles tests into a test assembly. This implementation is basically a wrapper around the CodeDomProvider,
    ///  and doesn't do much beyond that.
    /// </summary>
    public sealed class TestCompiler : ITestCompiler
    {
        #region Fields & Constants

        private readonly CodeDomProvider compiler;
        private readonly IList<string> referencePaths;
        private readonly IAssemblyResolver resolver;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the TestCompiler class.
        /// </summary>
        /// <param name="resolver">
        /// An IAssemblyResolver implementation to use when loading the compiled
        /// assemblies.
        /// </param>
        public TestCompiler(IAssemblyResolver resolver = null)
        {
            this.compiler = CodeDomProvider.CreateProvider("CSharp");
            this.referencePaths = new List<string>();
            this.resolver = resolver == null ? new AssemblyResolver() : resolver;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public IEnumerable<string> References
        {
            get
            {
                return this.referencePaths;
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public void AddReference(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Invalid reference path! It cannot be null or empty.");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found! Path: " + path);
            }

            if (!(Path.GetExtension(path).Equals(".dll", StringComparison.OrdinalIgnoreCase) ||
                   Path.GetExtension(path).Equals(".exe", StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Invalid file type! Only .dll and .exe are supported as references.");
            }

            // Made it. Whew.
            this.referencePaths.Add(Path.GetFullPath(path));
        }

        /// <inheritdoc />
        public void AddReferences(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                this.AddReference(path);
            }
        }

        /// <inheritdoc />
        public void ClearReferences()
        {
            this.referencePaths.Clear();
        }

        /// <inheritdoc />
        public IAssembly CompileTests(string sourcePath)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException("Invalid path.");
            }

            return this.CompileTests(sourcePath, Path.Combine(sourcePath, Path.GetRandomFileName() + ".dll"));
        }

        /// <inheritdoc />
        public IAssembly CompileTests(string sourcePath, string outputFileName)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException("Invalid path.");
            }

            var fullPath = Path.GetFullPath(sourcePath);
            if (Directory.Exists(fullPath))
            {
                var sourceFiles = this.GetSourceFiles(Path.GetFullPath(sourcePath));

                if (sourceFiles != null && sourceFiles.Length > 0)
                {
                    var compilerParameters = new CompilerParameters(
                        this.referencePaths.ToArray(),
                        outputFileName,
                        false);

                    var results = this.compiler.CompileAssemblyFromFile(compilerParameters, sourceFiles);

                    if (results.Errors.Count == 0)
                    {
                        // return the loaded assembly.
                        return resolver.LoadMeta(Path.GetFullPath(results.PathToAssembly));
                    }
                    else
                    {
                        throw new CompilationException(results);
                    }
                }

                // Empty directory, nothing to compile.
                return null;
            }

            throw new DirectoryNotFoundException("Directory not found. Path: " + fullPath);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Grabs a list of source files given a path.
        /// </summary>
        /// <param name="path">
        /// The directory to enumerate all the source files for.
        /// </param>
        /// <returns>
        /// A list of file paths.
        /// </returns>
        private string[] GetSourceFiles(string path)
        {
            string[] pathsList = null;

            if (Directory.Exists(path))
            {
                pathsList = Directory.GetFiles(path, "*.cs");
            }

            return pathsList;
        }

        #endregion
    }
}