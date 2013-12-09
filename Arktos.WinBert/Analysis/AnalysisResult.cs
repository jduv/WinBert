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

        public static readonly string NoDifferenceMessage = "Nothing interesting to report.";
        public static readonly string ExceptionMessage = "An exception occurred. Message: ";
        public static readonly string UnknownErrorMessage = "We're sorry, but an unknown error occurred.";
        public static readonly string SingleRunUnsuccessfulMessage = "A test run was unsuccessful! View logs for errors in the winbert root directory. Failed target location: ";
        public static readonly string BothRunsUnsuccessfulMessage = "Both test runs were unsuccessful! View logs for errors in the winbert root directory. Failed target locations: ";

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
                throw new ArgumentException("Bad input on unsuccessful test run factory method: both runs are successful!");
            }

            string message;
            if (!previousResults.Success && !currentResults.Success)
            {
                message = BothRunsUnsuccessfulMessage + System.Environment.NewLine + System.Environment.NewLine +
                    currentResults.Target.Location + System.Environment.NewLine + previousResults.Target.Location;
            }
            else
            {
                message = SingleRunUnsuccessfulMessage + System.Environment.NewLine + System.Environment.NewLine +
                        (previousResults.Success ? currentResults.Target.Location : previousResults.Target.Location);
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
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            var message = ExceptionMessage + exception.Message;
            return new InconclusiveAnalysisResult(message);
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