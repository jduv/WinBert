namespace Arktos.WinBert.Instrumentation
{
    using Arktos.WinBert.Differencing;
    using Arktos.WinBert.Testing;
    using Microsoft.Cci;

    /// <summary>
    /// Represents a test suite that has been instrumented.
    /// </summary>
    public class InstrumentedTestSuite : IRegressionTestSuite
    {
        #region Fields & Constants

        private readonly IRegressionTestSuite sourceSuite = null;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the InstrumentedTestSuite class.
        /// </summary>
        public InstrumentedTestSuite()
        {
        }

        /// <summary>
        /// Initializes a new instance of the InstrumentedTestSuite class.
        /// </summary>
        /// <param name="toCopy">
        /// A TestSuite to copy.
        /// </param>
        public InstrumentedTestSuite(IRegressionTestSuite toCopy)
        {
            this.sourceSuite = toCopy;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets an instrumented copy of the new assembly.
        /// </summary>
        public IAssembly InstrumentedNewTargetAssembly { get; set; }

        /// <summary>
        ///   Gets or sets an instrumented copy of the TestAssembly for the new assembly.
        /// </summary>
        public IAssembly InstrumentedNewTargetTestAssembly { get; set; }

        /// <summary>
        ///   Gets or sets an instrumented copy of the old assembly.
        /// </summary>
        public IAssembly InstrumentedOldTargetAssembly { get; set; }

        /// <summary>
        ///   Gets or sets an instrumented copy of the TestAssembly for the old assembly.
        /// </summary>
        public IAssembly InstrumentedOldTargetTestAssembly { get; set; }

        /// <summary>
        ///   Gets the difference result between the old and new assemblies.
        /// </summary>
        public IAssemblyDifferenceResult Diff
        {
            get
            {
                return this.sourceSuite.Diff;
            }
        }

        /// <summary>
        ///   Gets the new assembly.
        /// </summary>
        public IAssembly NewTargetAssembly
        {
            get
            {
                return this.InstrumentedNewTargetAssembly;
            }
        }

        /// <summary>
        ///   Gets a list of tests for the new assembly.
        /// </summary>
        public IAssembly NewTargetTestAssembly
        {
            get
            {
                return this.InstrumentedNewTargetTestAssembly;
            }
        }

        /// <summary>
        ///   Gets the old assembly.
        /// </summary>
        public IAssembly OldTargetAssembly
        {
            get
            {
                return this.InstrumentedOldTargetAssembly;
            }
        }

        /// <summary>
        ///   Gets a list of tests for the old assembly.
        /// </summary>
        public IAssembly OldTargetTestAssembly
        {
            get
            {
                return this.InstrumentedOldTargetTestAssembly;
            }
        }

        #endregion
    }
}