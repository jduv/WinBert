namespace Arktos.WinBert.Analysis
{
    using Arktos.WinBert.Testing;
    using System;

    /// <summary>
    /// Describes a run of the WinBert analysis engine.
    /// </summary>
    public abstract class AnalysisResult
    {
        #region Fields & Constants

        private static readonly string NoDifferenceMessage = "Nothing interesting to report.";
        private static readonly string ExceptionMessage = "An exception occurred. Message: {0}";
        private static readonly string UnknownErrorMessage = "We're sorry, but an unknown error occurred.";
        private static readonly string UnsuccessfulRunMessage = "A test run was unsuccessful! View logs for errors in the winbert root directory. Failed target location: " +
            System.Environment.NewLine + System.Environment.NewLine + "{0}";
        private static readonly string BothTestRunsBadMessage = "Both test runs were unsuccessful! View logs for errors in the winbert root directory. Failed target locations: " +
            System.Environment.NewLine + System.Environment.NewLine + "{0}" + System.Environment.NewLine + "{1}";

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates an analysis result communicating that an unsuccessful test run occurred.
        /// </summary>
        /// <param name="previousResults">
        /// The previous run that may have failed.
        /// </param>
        /// <returns></returns>
        public static AnalysisResult UnsuccessfulTestRun(ITestRunResult previousResults, ITestRunResult currentResults)
        {
            if (previousResults == null)
            {
                throw new ArgumentNullException("previousResults");
            }

            if (currentResults == null)
            {
                throw new ArgumentNullException("currentResults");
            }

            if (previousResults.Success && currentResults.Success)
            {
                throw new ArgumentException("Bad input: runs are successful! Unable to create the analysis result.");
            }

            string message;
            if (!previousResults.Success && !currentResults.Success)
            {
                message = string.Format(BothTestRunsBadMessage, currentResults.Target.Location, previousResults.Target.Location);
            }
            else
            {
                message = string.Format(UnsuccessfulRunMessage, previousResults.Success ? currentResults.Target.Location : previousResults.Target.Location);
            }

            return new InconclusiveAnalysisResult(message);
        }

        /// <summary>
        /// Creates an analysis result corresponding to no difference between builds.
        /// </summary>
        /// <returns>
        /// A new analysis result representing no difference between builds.
        /// </returns>
        public static AnalysisResult NoDifference()
        {
            return new InconclusiveAnalysisResult(NoDifferenceMessage);
        }

        /// <summary>
        /// Creates an analysis result from a target exception.
        /// </summary>
        /// <param name="exception">
        /// The exception to create the result from.
        /// </param>
        /// <returns>
        /// A new analysis result representing an error that occurred.
        /// </returns>
        public static AnalysisResult FromException(Exception exception)
        {
            var msg = string.Format(ExceptionMessage, exception.Message);
            return new InconclusiveAnalysisResult(msg);
        }

        /// <summary>
        /// Creates an analysis result specifying that an unknown error has occurred.
        /// </summary>
        /// <param name="message">
        /// The message for the error.
        /// </param>
        /// <returns>
        /// A new analysis result specifying that an unknown error occured.
        /// </returns>
        public static AnalysisResult UnknownError()
        {
            return new InconclusiveAnalysisResult(UnknownErrorMessage);
        }

        #endregion
    }
}