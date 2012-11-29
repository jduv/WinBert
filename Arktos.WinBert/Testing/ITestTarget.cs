namespace Arktos.WinBert.Testing
{
    using AppDomainToolkit;

    /// <summary>
    /// Defines a simple implementation that links a set of assemblies to the generated tests.
    /// </summary>
    public interface ITestTarget
    {
        #region Properties

        /// <summary>
        /// Gets the new assembly to test.
        /// </summary>
        IAssemblyTarget TargetNewAssembly { get; }

        /// <summary>
        /// Gets the old assembly to test.
        /// </summary>
        IAssemblyTarget TargetOldAssembly { get; }

        /// <summary>
        /// Gets the assembly containing tests for both targets.
        /// </summary>
        IAssemblyTarget TestAssembly { get; }

        #endregion
    }
}
