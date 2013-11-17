namespace Arktos.WinBert.Analysis
{
    using System;
    using Arktos.WinBert.Differencing;
    using Arktos.WinBert.Testing;

    /// <summary>
    /// Performs a basic behavioral analysis on the results from an instrumented test run.
    /// </summary>
    public class BertBehavioralAnalyzer : IBehavioralAnalyzer
    {
        #region Public Methods

        /// <inheritdoc />
        public AnalysisResult Analyze(IAssemblyDifferenceResult diff, ITestRunResult previousResults, ITestRunResult currentResults)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
