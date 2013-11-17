namespace Arktos.WinBert.Analysis
{
    using Arktos.WinBert.Differencing;
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
        /// <param name="diff">
        /// The difference between assemblies.
        /// </param>
        /// <param name="previousResults">
        /// The test results for the previous build.
        /// </param>
        /// <param name="currentResults">
        /// The test results for the current build.
        /// </param>
        /// <returns>
        /// An analysis result.
        /// </returns>
        AnalysisResult Analyze(IAssemblyDifferenceResult diff, ITestRunResult previousResults, ITestRunResult currentResults);

        #endregion
    }
}
