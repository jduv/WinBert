namespace Arktos.WinBert.VsPackage.ViewModel
{
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.Differencing;
    using System;
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
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            this.Differences = new ObservableCollection<TestExecutionDifference>(result.Differences);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an observable collection of the differences presented by this vm.
        /// </summary>
        public ObservableCollection<TestExecutionDifference> Differences { get; private set; }

        #endregion
    }
}
