namespace Arktos.WinBert.Environment
{
    using System.Reflection;

    /// <summary>
    /// Defines behavior for a loaded assembly target.
    /// </summary>
    public interface ILoadedAssemblyTarget : IAssemblyTarget
    {
        #region Properties

        /// <summary>
        /// Gets the assembly.
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        /// Gets the environment where the assembly is loaded.
        /// </summary>
        IAssemblyEnvironment Environment { get; }

        #endregion
    }
}
