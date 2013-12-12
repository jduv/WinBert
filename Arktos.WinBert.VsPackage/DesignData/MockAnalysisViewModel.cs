namespace Arktos.WinBert.VsPackage.DesignData
{
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.Differencing;
    using Arktos.WinBert.VsPackage.ViewModel;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Mock view model for displaying design time data. Enables testing without running the solution.
    /// </summary>
    public class MockAnalysisViewModel : AnalysisToolWindowVm
    {
        #region Constructors & Destructors

        public MockAnalysisViewModel()
        {
            this.AnalysisResults.Add(new InconclusiveAnalysisVm(new InconclusiveAnalysisResult("Inconclusive"), "Inconclusive Project"));
            this.AnalysisResults.Add(new AnalysisErrorInfoVm("An error occurred!", "My Bad Project"));
            this.AnalysisResults.Add(new AnalysisErrorInfoVm(new ArgumentNullException("myParam"), "Exception Thrown"));
            this.AnalysisResults.Add(new SuccessfulAnalysisVm(CreateSuccessfulAnalysisResult(), "Successful Project"));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Factory method that creates a successful analysis result for consumption of the UI design
        /// time view.
        /// </summary>
        /// <returns>
        /// A mock successful analysis result. Used for design time tweaks of the UI.
        /// </returns>
        private static SuccessfulAnalysisResult CreateSuccessfulAnalysisResult()
        {
            var diffs = new List<TestExecutionDifference>()
            {
            };

            return new SuccessfulAnalysisResult(diffs);
        }

        #endregion
    }
}
