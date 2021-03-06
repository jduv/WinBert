﻿namespace Arktos.WinBert.Testing
{
    using AppDomainToolkit;
    using System;
    using System.IO;

    /// <summary>
    /// Contains information about a test run. 
    /// </summary>
    [Serializable]
    public sealed class TestRunResult : ITestRunResult
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

        /// <inheritdoc />
        public bool Success { get; private set; }

        /// <inheritdoc />
        public string PathToAnalysisLog { get; private set; }

        /// <inheritdoc />
        public IAssemblyTarget Target { get; private set; }

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
        public static TestRunResult Successful(string pathToAnalysisLog, IAssemblyTarget target)
        {
            if (string.IsNullOrEmpty(pathToAnalysisLog))
            {
                throw new ArgumentException("Path to analysis log cannot be null or empty!");
            }

            if (!File.Exists(pathToAnalysisLog))
            {
                throw new ArgumentException("The target file doesn't exist! Path: " + pathToAnalysisLog);
            }

            if (target == null)
            {
                throw new ArgumentException("Target cannot be null!");
            }

            return new TestRunResult()
            {
                Success = true,
                PathToAnalysisLog = pathToAnalysisLog,
                Target = target
            };
        }

        /// <summary>
        /// Creates a new failed TestRunResult. This will set the Success property to false and
        /// the path to the analysis log to null.
        /// </summary>
        /// <returns>
        /// A new TestRunResult instance.
        /// </returns>
        public static TestRunResult Failure(IAssemblyTarget target)
        {

            if (target == null)
            {
                throw new ArgumentException("Target cannot be null!");
            }

            return new TestRunResult()
            {
                Success = false,
                PathToAnalysisLog = null,
                Target = target
            };
        }

        #endregion
    }
}
