namespace Arktos.WinBert.VsPackage.ViewModel
{
    using GalaSoft.MvvmLight;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This vm is responsible for holding the list of analysis results that will be rendered in the
    /// tool window.
    /// </summary>
    public class AnalysisToolWindowVm : ViewModelBase
    {
        #region Constructors & Destructors

        public AnalysisToolWindowVm()
        {
            this.AnalysisResults = new ObservableCollection<ViewModelBase>();
        }

        #endregion

        #region Properties

        public ObservableCollection<ViewModelBase> AnalysisResults { get; private set; }

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
