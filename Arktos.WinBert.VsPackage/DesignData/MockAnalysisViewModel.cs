namespace Arktos.WinBert.VsPackage.DesignData
{
    using Arktos.WinBert.Analysis;
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

        private static SuccessfulAnalysisResult CreateSuccessfulAnalysisResult()
        {
            var diffs = new List<BehavioralDifference>()
            {
               new BehavioralDifference(),
               new BehavioralDifference(),
               new BehavioralDifference()
            };

            return new SuccessfulAnalysisResult(diffs);
        }

        #endregion
    }
}
