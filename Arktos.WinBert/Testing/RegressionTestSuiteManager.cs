namespace Arktos.WinBert.Testing
{
    using System;
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.Differencing;
    using Arktos.WinBert.Xml;
    using Arktos.WinBert.Environment;
    using Arktos.WinBert.Instrumentation;

    /// <summary>
    /// The class that ties everything together. An implementation of this should be able to manage
    /// pulling together all the miscellaneous pieces required to build out a regression test suite and execute
    /// it, returning an analysis result.
    /// </summary>
    public class RegressionTestSuiteManager
    {
        #region Fields & Constants

        private readonly ITestGenerator generator;
        private readonly ITestInstrumenter instrumenter;
        private readonly ITestRunner runner;
        private readonly IBehavioralAnalyzer analyzer;
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
            ITestInstrumenter instrumenter,
            ITestRunner runner,
            IBehavioralAnalyzer analyzer)
        {
            if (config == null)
            {
                throw new ArgumentNullException("Config cannot be null.");
            }

            if (generator == null)
            {
                throw new ArgumentNullException("Test generator cannot be null.");
            }

            if (instrumenter == null)
            {
                throw new ArgumentNullException("Instrumenter cannot be null.");
            }

            if (runner == null)
            {
                throw new ArgumentNullException("Test runner cannot be null.");
            }

            if (analyzer == null)
            {
                throw new ArgumentNullException("Analyzer cannot be null.");
            }

            this.config = config;
            this.generator = generator;
            this.instrumenter = instrumenter;
            this.runner = runner;
            this.analyzer = analyzer;
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
            var currentBuildEnv = new AssemblyEnvironment();
            var currentAssembly = currentBuildEnv.LoadFile(current.AssemblyPath);

            var previousBuildEnv = new AssemblyEnvironment();
            var previousAssembly = previousBuildEnv.LoadFile(previous.AssemblyPath);

            var diff = this.DoDiff(currentAssembly, previousAssembly);
            if (diff.IsDifferent)
            {

            }
        }

        #endregion

        #region Protected Methods

        public IAssemblyDifferenceResult DoDiff(Assembly currentAssembly, Assembly previousAssembly)
        {
            return null;
        }

        #endregion
    }
}
