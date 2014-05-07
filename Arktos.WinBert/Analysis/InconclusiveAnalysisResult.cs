
namespace Arktos.WinBert.Analysis
{
    /// <summary>
    /// Represents an inconclusive analysis pass.
    /// </summary>
    public class InconclusiveAnalysisResult : AnalysisResult
    {
        #region Constructors & Destructors

        public InconclusiveAnalysisResult(string message)
            : base(false)
        {
            this.Reason = message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a message to the user.
        /// </summary>
        public string Reason { get; private set; }

        #endregion
    }
}
