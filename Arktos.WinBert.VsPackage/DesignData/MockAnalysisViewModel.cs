namespace Arktos.WinBert.VsPackage.DesignData
{
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.VsPackage.ViewModel;

    /// <summary>
    /// Mock view model for displaying design time data. Enables testing without running the solution.
    /// </summary>
    public class MockAnalysisViewModel : AnalysisToolWindowVm
    {
        #region Constructors & Destructors

        public MockAnalysisViewModel()
        {
            this.AnalysisResults.Add(new InconclusiveAnalysisVm(new InconclusiveAnalysisResult("Inconclusive"), "My Project 1"));
        }

        #endregion
    }
}
