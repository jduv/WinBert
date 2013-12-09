namespace Arktos.WinBert.VsPackage.ViewModel
{
    using GalaSoft.MvvmLight;
    using System.Collections.ObjectModel;

    /// <summary>
    /// This vm is responsible for holding the list of analysis results that will be rendered in the
    /// tool window.
    /// </summary>
    public class AnalysisToolWindowVm : ViewModelBase
    {
        #region Fields & Constants

        private int selectedIndex;
        private ObservableCollection<ViewModelBase> analysisResults;

        #endregion

        #region Constructors & Destructors

        public AnalysisToolWindowVm()
        {
            this.AnalysisResults = new ObservableCollection<ViewModelBase>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the list of analysis results view models to display.
        /// </summary>
        public ObservableCollection<ViewModelBase> AnalysisResults
        {
            get
            {
                return this.analysisResults;
            }

            private set
            {
                base.Set("AnalysisResults", ref analysisResults, value);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears all results currently in the VM.
        /// </summary>
        public void Clear()
        {
            this.AnalysisResults.Clear();
        }

        #endregion
    }
}
