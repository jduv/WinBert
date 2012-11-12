namespace Arktos.WinBert.Testing
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.Differencing;
    using Arktos.WinBert.Xml;

    /// <summary>
    /// The abstrct class that ties everything together. An implementation of this should be able to manage
    /// pulling together all the miscellaneous pieces required to build out a regression test suite and execute
    /// it, returning an analysis result. This class extends from MarshalByRefObject so that instances can be
    /// remoted into other application domains.
    /// </summary>
    public abstract class RegressionTestSuiteManager : MarshalByRefObject
    {
        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the RegressionTestSuiteManager class.
        /// </summary>
        /// <param name="config">
        /// The configuration.
        /// </param>
        public RegressionTestSuiteManager(WinBertConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("Config cannot be null.");
            }

            this.Config = config;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the currently set configuration
        /// </summary>
        protected WinBertConfig Config { get; private set; }

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
            var diff = this.DoDiff(current, previous);
            if (diff != null && diff.IsDifferent)
            {
                try
                {
                    var suite = this.BuildTestSuite(current, previous, diff);
                    suite = this.InstrumentTestSuite(suite);
                    result = this.ExecuteTestSuite(suite);
                }
                catch (Exception)
                {
                    // BMK Handle exception here.
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Loads an assembly without locking it or the PDB.
        /// </summary>
        /// <param name="path">
        /// The path of the assembly to load.
        /// </param>
        /// <returns>
        /// The assembly.
        /// </returns>
        protected static Assembly LoadAssembly(string assemblyPath, string pdbPath)
        {
            byte[] assemblyBytes = File.ReadAllBytes(assemblyPath);
            byte[] pdbBytes = string.IsNullOrEmpty(pdbPath) ? null : File.ReadAllBytes(pdbPath);
            return Assembly.Load(assemblyBytes, pdbBytes);
        }

        /// <summary>
        /// Executes the target test suite.
        /// </summary>
        /// <param name="toExecute">
        /// The test suite to execute.
        /// </param>
        /// <returns>
        /// An analysis result.
        /// </returns>
        protected abstract AnalysisResult ExecuteTestSuite(IRegressionTestSuite toExecute);

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
        protected abstract IRegressionTestSuite BuildTestSuite(Build current, Build previous, AssemblyDifferenceResult diff);

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
        /// <param name="current">The current build.</param>
        /// <param name="previous">The previous build.</param>
        /// <returns></returns>
        protected AssemblyDifferenceResult DoDiff(Build current, Build previous)
        {
            var differ = new BertAssemblyDifferenceEngine(this.Config.IgnoreList);

            try
            {
                var currentAssembly = LoadAssembly(current.AssemblyPath, current.PdbPath);
                var previousAssembly = LoadAssembly(previous.AssemblyPath, previous.PdbPath);
                return differ.Diff(previousAssembly, currentAssembly);
            }
            catch (Exception exception)
            {
                Trace.TraceError("Unable to diff assemblies {0} and {1}. Exception: {2}", previous.AssemblyPath, current.AssemblyPath, exception);
            }

            return null;
        }

        #endregion
    }
}
