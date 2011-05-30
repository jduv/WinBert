namespace WinBert.RandoopIntegration.Tests.Mocks
{
    using System.Reflection;
    using WinBert.Differencing;
    using WinBert.Testing;

    /// <summary>
    /// The mock test suite.
    /// </summary>
    public class MockTestSuite : ITestSuite
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MockTestSuite"/> class.
        /// </summary>
        /// <param name="newTests">
        /// The new tests.
        /// </param>
        /// <param name="oldTests">
        /// The old tests.
        /// </param>
        /// <param name="diff">
        /// The diff.
        /// </param>
        public MockTestSuite(string newTests, string oldTests, IAssemblyDifferenceResult diff)
        {
            this.Diff = diff;
            this.NewTargetTestAssembly = Assembly.LoadFile(newTests);
            this.OldTargetTestAssembly = Assembly.LoadFile(oldTests);
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets NewTargetTestAssembly.
        /// </summary>
        public Assembly NewTargetTestAssembly
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets OldTargetTestAssembly.
        /// </summary>
        public Assembly OldTargetTestAssembly
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets OldTargetAssembly.
        /// </summary>
        public Assembly OldTargetAssembly
        {
            get { return this.Diff.OldObject; }
        }

        /// <summary>
        /// Gets NewTargetAssembly.
        /// </summary>
        public Assembly NewTargetAssembly
        {
            get 
            { 
                return this.Diff.NewObject; 
            }
        }

        /// <summary>
        /// Gets Diff.
        /// </summary>
        public IDifferenceResult<Assembly> Diff
        {
            get;
            private set;
        }

        #endregion
    }
}
