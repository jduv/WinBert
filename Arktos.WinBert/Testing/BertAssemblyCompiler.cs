namespace Arktos.WinBert.Testing
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Compiles tests into a test assembly. This implementation is basically a wrapper around the CodeDomProvider,
    ///   and doesn't do much beyond that. It can easily be extended for more specific scenarios through sub classing.
    /// </summary>
    public class BertAssemblyCompiler : IAssemblyCompiler
    {
        #region Constants and Fields

        /// <summary>
        ///   The CodeDom compiler for compiling generated tests.
        /// </summary>
        private readonly CodeDomProvider compiler = null;

        /// <summary>
        ///   A list of reference paths.
        /// </summary>
        private readonly IList<string> referencePaths = new List<string>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the BertAssemblyCompiler class.
        /// </summary>
        /// <param name="outputPath">
        /// The output directory for the compiled assembly.
        /// </param>
        public BertAssemblyCompiler(string outputPath)
        {
            this.compiler = CodeDomProvider.CreateProvider("CSharp");

            this.InitializeOutputPath(outputPath);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a list of reference paths for the compiler to include during compilation.
        /// </summary>
        public IEnumerable<string> References
        {
            get
            {
                return this.referencePaths;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a reference assembly to the list of assemblies to include during compilation.
        /// </summary>
        /// <param name="path">
        /// The path to the reference.
        /// </param>
        public void AddReference(string path)
        {
            // ensure that the file exists and it's a library or executable
            if (File.Exists(path) && (Path.GetExtension(path).Equals(".dll") || Path.GetExtension(path).Equals(".exe")))
            {
                this.referencePaths.Add(Path.GetFullPath(path));
            }
        }

        /// <summary>
        /// Clears the reference path cache for this compiler.
        /// </summary>
        public void ClearReferences()
        {
            this.referencePaths.Clear();
        }

        /// <summary>
        /// Compiles tests.
        /// </summary>
        /// <param name="sourcePath">
        /// The source path to compile from.
        /// </param>
        /// <returns>
        /// A test assembly containing the compiled source or null on error.
        /// </returns>
        public Assembly CompileTests(string sourcePath)
        {
            if (Directory.Exists(sourcePath))
            {
                var sourceFiles = this.GetSourceFiles(Path.GetFullPath(sourcePath));

                CompilerResults results = null;
                if (sourceFiles != null && sourceFiles.Length > 0)
                {
                    var compilerParameters = new CompilerParameters(
                        this.referencePaths.ToArray(), this.GetRandomOutputFileName(), false);

                    results = this.compiler.CompileAssemblyFromFile(compilerParameters, sourceFiles);

                    // try and load the compiled assembly.
                    try
                    {
                        var assembly = Assembly.LoadFile(Path.GetFullPath(results.PathToAssembly));
                        return assembly;
                    }
                    catch (Exception)
                    {
                        // BMK handle exception
                    }
                }
            }

            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a random file name with a ".dll" extension.
        /// </summary>
        /// <returns>
        /// A random file name with the ".dll" extension.
        /// </returns>
        private string GetRandomOutputFileName()
        {
            return Path.GetRandomFileName() + ".dll";
        }

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

        /// <summary>
        /// Initializes the working directory of the compiler.
        /// </summary>
        /// <param name="pathToWorkingDirectory">
        /// The path to the working directory that will be used during compilation.
        /// </param>
        private void InitializeOutputPath(string pathToWorkingDirectory)
        {
            if (Directory.Exists(pathToWorkingDirectory))
            {
                Path.GetFullPath(pathToWorkingDirectory);
            }
        }

        #endregion
    }
}