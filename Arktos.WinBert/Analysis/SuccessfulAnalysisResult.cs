namespace Arktos.WinBert.Analysis
{
    using Arktos.WinBert.Differencing;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a successful analysis result.
    /// </summary>
    public class SuccessfulAnalysisResult : AnalysisResult
    {
        #region Constructors & Destructors

        public SuccessfulAnalysisResult(IEnumerable<TestExecutionDifference> differences)
        {
            this.Differences = differences;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of differences for view.
        /// </summary>
        public IEnumerable<TestExecutionDifference> Differences { get; private set; }

        #endregion
    }
}
