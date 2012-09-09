namespace Arktos.WinBert.Testing
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Defines behavior for a component that is able to compile an Assembly
    ///   from source located at the target path.
    /// </summary>
    public interface IAssemblyCompiler
    {
        #region Properties

        /// <summary>
        ///   Gets a list of the reference assemblies for this compiler to use when compiling
        ///   the target source.
        /// </summary>
        IEnumerable<string> References { get; }

        #endregion

        #region Interface Methods

        /// <summary>
        /// Adds a reference to the target Assembly or PE to this compiler.
        /// </summary>
        /// <param name="path">
        /// The path to the PE/Assembly to refer to.
        /// </param>
        void AddReference(string path);

        /// <summary>
        /// Adds a list of reference assemblies to the list of assemblies to include during compliation.
        /// </summary>
        /// <param name="paths">
        /// The paths to add.
        /// </param>
        void AddReferences(IEnumerable<string> paths);

        /// <summary>
        /// Clears all references.
        /// </summary>
        void ClearReferences();

        /// <summary>
        /// Compiles all source at the target path into an Assembly.
        /// </summary>
        /// <param name="sourcePath">
        /// The path to the source to compile.
        /// </param>
        /// <returns>
        /// A TestAssembly, or null on failure.
        /// </returns>
        Assembly CompileTests(string sourcePath);

        #endregion
    }
}