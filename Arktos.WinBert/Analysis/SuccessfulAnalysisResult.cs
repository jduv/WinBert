namespace Arktos.WinBert.Analysis
{
    using System.Collections.Generic;

    public class SuccessfulAnalysisResult : AnalysisResult
    {
        public SuccessfulAnalysisResult(IEnumerable<BehavioralDifference> differences)
        {
            this.Differences = differences;
        }

        public IEnumerable<BehavioralDifference> Differences { get; private set; }
    }
}
