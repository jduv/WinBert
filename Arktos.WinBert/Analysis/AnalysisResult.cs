namespace Arktos.WinBert.Analysis
{
    using System;

    /// <summary>
    /// Describes a run of the WinBert analysis engine.
    /// </summary>
    public class AnalysisResult
    {
        #region Public Methods

        public AnalysisResult NoDifference()
        {
            return null;
        }

        public AnalysisResult Error(Exception exc)
        {
            return null;
        }

        public AnalysisResult Error(string message)
        {
            return null;
        }

        #endregion
    }
}