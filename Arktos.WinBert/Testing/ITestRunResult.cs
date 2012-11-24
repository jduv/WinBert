namespace Arktos.WinBert.Testing
{
    /// <summary>
    /// Defines behavior for implementations represent test run results.
    /// </summary>
    public interface ITestRunResult
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this test run restult was successful or not.
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// The path to the analysis log.
        /// </summary>
        string PathToAnalysisLog { get; }

        #endregion
    }
}
