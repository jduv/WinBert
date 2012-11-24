namespace Arktos.WinBert.Testing
{
    using AppDomainToolkit;

    /// <summary>
    /// Defines a simple implementation that links an assembly to it's tests.
    /// </summary>
    public interface ITestTarget
    {
        #region Properties

        /// <summary>
        /// Gets the assembly to test.
        /// </summary>
        IAssemblyTarget TargetAssembly { get; }

        /// <summary>
        /// Gets the assembly containing tests for the target.
        /// </summary>
        IAssemblyTarget TestAssembly { get; }

        #endregion
    }
}
