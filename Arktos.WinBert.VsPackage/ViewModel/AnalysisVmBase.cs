namespace Arktos.WinBert.VsPackage.ViewModel
{
    using GalaSoft.MvvmLight;

    /// <summary>
    /// Base view model for all analysis types.
    /// </summary>
    public abstract class AnalysisVmBase : ViewModelBase
    {
        #region Constructors & Destructors

        public AnalysisVmBase(string projectName)
        {
            this.ProjectName = projectName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the project that was analyzed.
        /// </summary>
        public string ProjectName { get; private set; }

        #endregion
    }
}
