namespace Arktos.WinBert.Testing
{
    using System;
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.Differencing;
    using Arktos.WinBert.Util;
    using Arktos.WinBert.Xml;
    using Microsoft.Cci;
    using Arktos.WinBert.Environment;

    /// <summary>
    /// The class that ties everything together. An implementation of this should be able to manage
    /// pulling together all the miscellaneous pieces required to build out a regression test suite and execute
    /// it, returning an analysis result.
    /// </summary>
    public class RegressionTestSuiteManager
    {
        #region Fields & Constants

        private readonly ITestGenerator generator;
        private readonly ITestRunner runner;
        private readonly IMetaAssemblyResolver resolver;
        private readonly WinBertConfig config;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the RegressionTestSuiteManager class.
        /// </summary>
        /// <param name="config">The configuration to initialize with.</param>
        /// <param name="generator">The generator implementation to use when generating test assembiles.</param>
        /// <param name="runner">The test runner.</param>
        public RegressionTestSuiteManager(
            WinBertConfig config,
            ITestGenerator generator,
            ITestRunner runner)
        {
            if (config == null)
            {
                throw new ArgumentNullException("Config cannot be null.");
            }

            if (generator == null)
            {
                throw new ArgumentNullException("Test generator cannot be null.");
            }

            if (runner == null)
            {
                throw new ArgumentNullException("Test runner cannot be null.");
            }

            this.config = config;
            this.generator = generator;
            this.runner = runner;
            this.resolver = new MetaAssemblyResolver();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generates a test suite.
        /// </summary>
        /// <param name="current">
        /// The current build.
        /// </param>
        /// <param name="previous">
        /// The previous build.
        /// </param>
        /// <returns>A test suite, or null if something went wrong.</returns>
        public AnalysisResult BuildAndExecuteTestSuite(Build current, Build previous)
        {
            AnalysisResult result = null;
            try
            {
                var diff = this.DoDiff(current, previous);
                if (diff != null && diff.IsDifferent)
                {
                    var suite = this.BuildTestSuite(current, previous, diff);
                    suite = this.InstrumentTestSuite(suite);
                    result = this.ExecuteTestSuite(suite);
                }
                else
                {
                    // BMK Handle no results here.
                    result = null;
                }
            }
            catch (Exception)
            {
                // BMK Handle exception here.
            }

            return result;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Executes the target test suite.
        /// </summary>
        /// <param name="toExecute">
        /// The test suite to execute.
        /// </param>
        /// <returns>
        /// An analysis result.
        /// </returns>
        protected AnalysisResult ExecuteTestSuite(IRegressionTestSuite toExecute)
        {
            return this.runner.RunTests(toExecute);
        }

        /// <summary>
        /// Builds a test suite from the target builds.
        /// </summary>
        /// <param name="current">
        /// The current build.
        /// </param>
        /// <param name="previous">
        /// The previous build.
        /// </param>
        /// <param name="diff">
        /// The difference result.
        /// </param>
        /// <returns>A fully compiled regression test suite.</returns>
        protected IRegressionTestSuite BuildTestSuite(Build current, Build previous, IAssemblyDifferenceResult diff)
        {
            IRegressionTestSuite result = null;
            ////var types = diff.TypeDifferences.Select(x => x.NewObject).ToList();

            ////// Generate tests for the last tested build if we need to
            ////IAssembly previousBuildTests = string.IsNullOrEmpty(previous.TestAssemblyPath) ?
            ////    generator.GetTestsFor(diff.OldObject, types) :
            ////    resolver.LoadMeta(previous.AssemblyPath);

            ////// Generate tests for the newest build
            ////IAssembly currentBuildTests = generator.GetTestsFor(diff.NewObject, types);

            ////// If we have tests for both, we're good to go.
            ////if (previousBuildTests != null && currentBuildTests != null)
            ////{
            ////    current.TestAssemblyPath = currentBuildTests.Location;
            ////    previous.TestAssemblyPath = previousBuildTests.Location;
            ////    result = new RegressionTestSuite(currentBuildTests, previousBuildTests, diff);
            ////}

            return result;
        }

        /// <summary>
        /// Instruments the target test suite.
        /// </summary>
        /// <param name="toInstrument">
        /// The test suite to instrument.
        /// </param>
        /// <returns>
        /// An instrumented version of the passed in test suite.
        /// </returns>
        protected IRegressionTestSuite InstrumentTestSuite(IRegressionTestSuite toInstrument)
        {
            // Default implementation performs no instrumentation.
            return toInstrument;
        }

        /// <summary>
        /// Performs a diff between the two builds.
        /// </summary>
        /// <param name="current">
        /// The current build.
        /// </param>
        /// <param name="previous">
        /// The previous build.
        /// </param>
        /// <returns>
        /// A difference result.
        /// </returns>
        protected IAssemblyDifferenceResult DoDiff(Build current, Build previous)
        {
            var differ = new AssemblyDifferenceEngine(this.config.IgnoreList);
            var currentAssembly = new AssemblyResolver().LoadFile(current.AssemblyPath);
            var previousAssembly = new AssemblyResolver().LoadFile(previous.AssemblyPath);
            return differ.Diff(previousAssembly, currentAssembly);
        }

        #endregion
    }
}
