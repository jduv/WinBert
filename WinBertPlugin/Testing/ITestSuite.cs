namespace WinBert.Testing
{
    using System.Reflection;
    using WinBert.Differencing;

    /// <summary>
    /// Defines a basic property set that all TestSuite's should implement. This is mainly used to provide
    ///   a common functionality point for running tests.
    /// </summary>
    public interface ITestSuite
    {
        #region Properties

        /// <summary>
        ///   Gets the difference result.
        /// </summary>
        IDifferenceResult<Assembly> Diff { get; }

        /// <summary>
        ///   Gets the new assembly.
        /// </summary>
        Assembly NewTargetAssembly { get; }

        /// <summary>
        ///   Gets an assembly holding the tests for the new assembly target.
        /// </summary>
        Assembly NewTargetTestAssembly { get; }

        /// <summary>
        ///   Gets the old assembly.
        /// </summary>
        Assembly OldTargetAssembly { get; }

        /// <summary>
        ///   Gets an Assembly holding the tests for the old assembly target.
        /// </summary>
        Assembly OldTargetTestAssembly { get; }

        #endregion
    }
}