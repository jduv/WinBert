
namespace Arktos.WinBert.VsPackage.ViewModel
{
    using Arktos.WinBert.Analysis;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Handles display of a successful analysis result.
    /// </summary>
    public class SuccessfulAnalysisVm : AnalysisVmBase
    {
        #region Constructors & Destructors

        public SuccessfulAnalysisVm(SuccessfulAnalysisResult result, string projectName)
            : base(projectName)
        {
            this.AnalysisResult = result;
            this.Differences = new ObservableCollection<BehavioralDifference>(result.Differences);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an observable collection of the differences presented by this vm.
        /// </summary>
        public ObservableCollection<BehavioralDifference> Differences { get; private set; }

        /// <summary>
        /// Gets the original AnalysisResult used to build this vm.
        /// </summary>
        public SuccessfulAnalysisResult AnalysisResult { get; private set; }

        #endregion
    }
}
