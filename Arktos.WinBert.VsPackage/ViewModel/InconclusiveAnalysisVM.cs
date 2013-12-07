namespace Arktos.WinBert.VsPackage.ViewModel
{
    using Arktos.WinBert.Analysis;
    using GalaSoft.MvvmLight;

    /// <summary>
    /// A simple view model for displaying inconclusive analysis results as produced by the WinBert
    /// system.
    /// </summary>
    public class InconclusiveAnalysisVm : AnalysisVmBase
    {
        #region Fields & Constants

        private InconclusiveAnalysisResult result;

        #endregion

        #region Constructors & Destructors

        public InconclusiveAnalysisVm(InconclusiveAnalysisResult result)
        {
            this.result = result;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the result message.
        /// </summary>
        public string Message
        {
            get
            {
                return this.result.Message;
            }
        }

        #endregion
    }
}
