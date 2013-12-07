namespace Arktos.WinBert.Analysis
{
    /// <summary>
    /// Represents an inconclusive analysis result.
    /// </summary>
    public class InconclusiveAnalysisResult : AnalysisResult
    {
        #region Fields & Constants

        private readonly string message;

        #endregion

        #region Fields & Constants

        public InconclusiveAnalysisResult(string message)
        {
            this.message = message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the message to display to the user.
        /// </summary>
        public string Message
        {
            get
            {
                return this.message;
            }
        }

        #endregion
    }
}