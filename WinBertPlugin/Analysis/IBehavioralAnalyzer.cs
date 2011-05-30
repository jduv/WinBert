namespace WinBert.Analysis
{
    using WinBert.Testing;

    /// <summary>
    /// Defines the behavior of an implementation that can analyze behavioral differences
    ///   between two TestRunResult objects generated from the other steps in the behavioral
    ///   regression testing methodology.
    /// </summary>
    public interface IBehavioralAnalyzer
    {
        #region Public Methods

        /// <summary>
        /// Analyzes and compares the passed in TestSuiteRunResult object and returns an analysis results describing
        ///   their behavioral differences.
        /// </summary>
        /// <param name="results">
        /// The results of the test run to analyze.
        /// </param>
        /// <returns>
        /// An AnalysisResults object holding the results of the analysis.
        /// </returns>
        AnalysisResults Analyze(TestSuiteRunResult results);

        #endregion
    }
}