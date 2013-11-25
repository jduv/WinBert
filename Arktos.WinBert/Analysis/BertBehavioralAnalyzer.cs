namespace Arktos.WinBert.Analysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Arktos.WinBert.Differencing;
    using Arktos.WinBert.Testing;
    using Arktos.WinBert.Util;
    using Arktos.WinBert.Xml;

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
            AnalysisResult result;
            if (diff != null && previousResults != null && currentResults != null)
            {
                if (previousResults.Success && currentResults.Success)
                {
                    try
                    {
                        result = this.Process(diff, previousResults, currentResults);
                    }
                    catch (Exception e)
                    {
                        // Deal with exception
                        result = null;
                    }
                }
                else
                {
                    // Error with one of the results, report it.
                    result = null;
                }
            }
            else if (!diff.IsDifferent)
            {
                // No difference between runs.
                result = null;
            }
            else
            {
                // Unknown what happened, report a fatal error.
                result = null;
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

            return null;
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
