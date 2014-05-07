namespace Arktos.WinBert.Analysis
{
    using Arktos.WinBert.Differencing;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a successful analysis result.
    /// </summary>
    public class SuccessfulAnalysisResult : AnalysisResult
    {
        #region Constructors & Destructors

        public SuccessfulAnalysisResult(IEnumerable<ITestExecutionDifference> differences)
            : base(true)
        {
            if (differences == null)
            {
                throw new ArgumentException("Differences cannot be null.");
            }
            this.Differences = differences;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of differences for view.
        /// </summary>
        public IEnumerable<ITestExecutionDifference> Differences { get; private set; }

        #endregion
    }
}
