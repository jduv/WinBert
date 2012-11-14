namespace Arktos.WinBert.Testing
{
    using Arktos.WinBert.Differencing;
    using Microsoft.Cci;

    /// <summary>
    /// Defines a basic property set that all TestSuite's should implement. This is mainly used to provide
    /// a common functionality point for running tests.
    /// </summary>
    public interface IRegressionTestSuite
    {
        #region Properties

        /// <summary>
        /// Gets the difference result.
        /// </summary>
        IAssemblyDifferenceResult Diff { get; }

        /// <summary>
        /// Gets the new assembly.
        /// </summary>
        IAssembly NewTargetAssembly { get; }

        /// <summary>
        /// Gets an assembly holding the tests for the new assembly target.
        /// </summary>
        IAssembly NewTargetTestAssembly { get; }

        /// <summary>
        /// Gets the old assembly.
        /// </summary>
        IAssembly OldTargetAssembly { get; }

        /// <summary>
        /// Gets an Assembly holding the tests for the old assembly target.
        /// </summary>
        IAssembly OldTargetTestAssembly { get; }

        #endregion
    }
}