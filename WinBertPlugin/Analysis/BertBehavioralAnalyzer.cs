namespace WinBert.Analysis
{
    using System;
    using WinBert.Testing;

    /// <summary>
    /// A basic behavioral analyzer that will
    /// </summary>
    public class BertBehavioralAnalyzer : IBehavioralAnalyzer
    {
        #region Implemented Interfaces

        #region IBehavioralAnalyzer

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
        public AnalysisResults Analyze(TestSuiteRunResult results)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}