namespace Arktos.WinBert.Analysis
{
    using System.Collections.Generic;

    public class SuccessfulAnalysisResult : AnalysisResult
    {
        #region Constructors & Destructors

        public SuccessfulAnalysisResult(IEnumerable<BehavioralDifference> differences)
        {
            this.Differences = differences;
        }

        #endregion

        #region Properties

        public IEnumerable<BehavioralDifference> Differences { get; private set; }

        #endregion
    }
}
