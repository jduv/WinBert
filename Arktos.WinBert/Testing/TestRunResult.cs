namespace Arktos.WinBert.Testing
{
    using System;
    using System.IO;

    /// <summary>
    /// Contains information about a test run. 
    /// </summary>
    public sealed class TestRunResult
    {
        #region Constructors & Destructors

        /// <summary>
        /// Prevents a default instance of the TestRunResult from being created. Use Factory methods
        /// instead.
        /// </summary>
        private TestRunResult()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this test run restult was successful or not.
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// The path to the analysis log.
        /// </summary>
        public string PathToAnalysisLog { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new successful TestRunResult.
        /// </summary>
        /// <param name="pathToAnalysisLog">
        /// The path to the analysis log. This must exist--the method will double check you to 
        /// be sure. Expect exceptions if you pass this a bad path or malformed input.
        /// </param>
        /// <returns>
        /// A new TestRunResult instance.
        /// </returns>
        public static TestRunResult Successful(string pathToAnalysisLog)
        {
            if (string.IsNullOrEmpty(pathToAnalysisLog))
            {
                throw new ArgumentException("Path to analysis log cannot be null or empty!");
            }

            if (!File.Exists(pathToAnalysisLog))
            {
                throw new ArgumentException("The target file doesn't exist! Path: " + pathToAnalysisLog);
            }

            return new TestRunResult()
            {
                Success = true,
                PathToAnalysisLog = pathToAnalysisLog
            };
        }

        /// <summary>
        /// Creates a new failed TestRunResult. This will set the Success property to false and
        /// the path to the analysis log to null.
        /// </summary>
        /// <returns>
        /// A new TestRunResult instance.
        /// </returns>
        public static TestRunResult Failure()
        {
            return new TestRunResult()
            {
                Success = false,
                PathToAnalysisLog = null
            };
        }

        #endregion
    }
}
