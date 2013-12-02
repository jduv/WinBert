namespace Arktos.WinBert.VsPackage.ViewModel
{
    using Arktos.WinBert.Analysis;
    using GalaSoft.MvvmLight;

    /// <summary>
    /// A simple view model for displaying analysis results as produced by the WinBert
    /// system.
    /// </summary>
    public class AnalysisViewModel : ViewModelBase
    {
        #region Fields & Constants

        private AnalysisResult result;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Analysis result for the VM.
        /// </summary>
        public AnalysisResult Analysis
        {
            get
            {
                return this.result;
            }

            set
            {
                base.Set("Analysis", ref result, value);
            }
        }

        #endregion
    }
}
