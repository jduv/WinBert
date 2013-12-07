namespace Arktos.WinBert.VsPackage.ViewModel
{
    using Arktos.WinBert.Analysis;
    using System;

    /// <summary>
    /// Creates viewmodels. Standard factory pattern.
    /// </summary>
    public static class ViewModelFactory
    {
        #region Public Methods

        /// <summary>
        /// Creates a new view model based on the analysis result passed.
        /// </summary>
        /// <param name="result">
        /// The result to map to a view model.
        /// </param>
        /// <returns>
        /// A VM capable of presenting the target view.
        /// </returns>
        public static AnalysisVmBase Create(AnalysisResult result, string projectName)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            AnalysisVmBase vm;
            if (result is InconclusiveAnalysisResult)
            {
                vm = new InconclusiveAnalysisVm(result as InconclusiveAnalysisResult, projectName);
            }
            else
            {
                vm = new AnalysisErrorInfoVm("Internal Error: No view model mapped to the target AnalysisResult implementation!", projectName);
            }

            return vm;
        }

        #endregion
    }
}