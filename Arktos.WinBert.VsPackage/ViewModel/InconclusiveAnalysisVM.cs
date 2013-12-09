namespace Arktos.WinBert.VsPackage.ViewModel
{
    using Arktos.WinBert.Analysis;
    using System;

    /// <summary>
    /// A simple view model for displaying inconclusive analysis results as produced by the WinBert
    /// system.
    /// </summary>
    public class InconclusiveAnalysisVm : AnalysisVmBase
    {
        #region Constructors & Destructors

        public InconclusiveAnalysisVm(InconclusiveAnalysisResult result, string projectName)
            : base(projectName)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            this.Message = result.Message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the result message.
        /// </summary>
        public string Message { get; private set; }

        #endregion
    }
}
