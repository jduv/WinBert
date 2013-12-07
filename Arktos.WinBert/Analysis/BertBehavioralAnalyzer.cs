﻿namespace Arktos.WinBert.Analysis
{
    using Arktos.WinBert.Differencing;
    using Arktos.WinBert.Testing;
    using Arktos.WinBert.Util;
    using Arktos.WinBert.Xml;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Performs a basic behavioral analysis on the results from an instrumented test run.
    /// </summary>
    public class BertBehavioralAnalyzer : IBehavioralAnalyzer
    {
        #region Fields & Constants

        private readonly IFileSystem fileSystem;

        #endregion

        #region Constructors & Destructors

        public BertBehavioralAnalyzer()
            : this(new FileSystem())
        {
        }

        public BertBehavioralAnalyzer(IFileSystem fileSystem)
        {
            if (fileSystem == null)
            {
                throw new ArgumentNullException("Filesystem cannot be null.");
            }

            this.fileSystem = fileSystem;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public AnalysisResult Analyze(IAssemblyDifferenceResult diff, ITestRunResult previousResults, ITestRunResult currentResults)
        {
            if (diff == null)
            {
                throw new ArgumentNullException("diff");
            }

            if (previousResults == null)
            {
                throw new ArgumentNullException("previousResults");
            }

            if (currentResults == null)
            {
                throw new ArgumentNullException("currentResults");
            }

            AnalysisResult result;
            if (!diff.IsDifferent)
            {
                // No difference between runs.
                result = AnalysisResult.NoDifference();
            }
            else if (!(previousResults.Success && currentResults.Success))
            {
                result = AnalysisResult.UnsuccessfulTestRun(previousResults, currentResults);
            }
            else
            {
                try
                {
                    result = this.Process(diff, previousResults, currentResults);
                }
                catch (Exception exception)
                {
                    // Deal with exception
                    result = AnalysisResult.FromException(exception);
                }
            }
            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads the analysis log at the target path.
        /// </summary>
        /// <param name="path">
        /// The path of the analysis log to load.
        /// </param>
        /// <returns>
        /// An analysis log instance.
        /// </returns>
        private WinBertAnalysisLog LoadAnalysisLog(string path)
        {
            var file = this.fileSystem.OpenRead(path);
            return Serializer.XmlDeserialize<WinBertAnalysisLog>(file);
        }

        /// <summary>
        /// Private implementation of Analyze. Assumes all inputs are sane and that the passed in difference
        /// result has types that are indeed different.
        /// </summary>
        /// <param name="diff">
        /// The difference result. Assumed to not be null.
        /// </param>
        /// <param name="previousResults">
        /// The previous test results. Assumed to not be null.
        /// </param>
        /// <param name="currentResults">
        /// The current test results. Assumed to not be null.
        /// </param>
        /// <returns>
        /// Returns an analysis result.
        /// </returns>
        private AnalysisResult Process(IAssemblyDifferenceResult diff, ITestRunResult previousResults, ITestRunResult currentResults)
        {
            var typeDifferences = diff.TypeDifferences.ToDictionary(x => x.Name, y => new TypeDifferenceLookup(y));
            var previousLog = LoadAnalysisLog(previousResults.PathToAnalysisLog);
            var currentLog = LoadAnalysisLog(currentResults.PathToAnalysisLog);

            // TODO: Fix me.
            return new InconclusiveAnalysisResult("Valid Analysis!");
        }

        #endregion

        #region Private Classes

        /// <summary>
        /// Small utiltity type representing a Type along with it's set of method names that
        /// changed. Use this for fast lookup of type to method name relationships.
        /// </summary>
        private class TypeDifferenceLookup
        {
            #region Fields & Constants

            private readonly HashSet<string> methodNames;

            #endregion

            #region Constructors & Destructors

            public TypeDifferenceLookup(ITypeDifferenceResult typeDiff)
            {
                this.methodNames = new HashSet<string>();
                foreach (var method in typeDiff.Methods)
                {
                    this.methodNames.Add(method);
                }

                this.TypeName = typeDiff.FullName;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the parent type's name.
            /// </summary>
            public string TypeName { get; private set; }

            #endregion

            #region Public Methods

            /// <summary>
            /// 
            /// </summary>
            /// <param name="methodSignature"></param>
            /// <returns></returns>
            public bool Contains(string methodSignature)
            {
                return this.methodNames.Contains(methodSignature);
            }

            #endregion
        }

        #endregion
    }
}
