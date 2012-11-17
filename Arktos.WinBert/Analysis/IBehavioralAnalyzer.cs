namespace Arktos.WinBert.Analysis
{
    using Arktos.WinBert.Testing;

    /// <summary>
    /// Defines behavior for implementations that perform behavioral analysis on log files gleaned
    /// from test runs of instrumented assemblies.
    /// </summary>
    public interface IBehavioralAnalyzer
    {
        #region Methods

        /// <summary>
        /// Performs basic behavioral analysis on the target test run results.
        /// </summary>
        /// <param name="currentBuildResults">
        /// The test results for the current build.
        /// </param>
        /// <param name="previousBuildResults">
        /// The test results for the previous build.
        /// </param>
        /// <returns>
        /// An analysis result.
        /// </returns>
        AnalysisResult Analyze(TestRunResult currentBuildResults, TestRunResult previousBuildResults);

        #endregion
    }
}
