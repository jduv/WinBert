namespace Arktos.WinBert.VsPackage.ViewModel
{
    using Arktos.WinBert.Analysis;
    using GalaSoft.MvvmLight;
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
        public static ViewModelBase Create(AnalysisResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            ViewModelBase vm;
            if (result is InconclusiveAnalysisResult)
            {
                vm = new InconclusiveAnalysisVm(result as InconclusiveAnalysisResult);
            }
            else
            {
                vm = new ErrorInfoVm("Internal Error: No view model mapped to the target AnalysisResult implementation!");
            }

            return vm;
        }

        #endregion
    }
}