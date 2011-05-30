namespace WinBert.RandoopIntegration.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the RandoopTestRunner class.
    /// </summary>
    [TestClass]
    public class RandoopTestRunnerTest
    {
        #region Constants and Fields

        /// <summary>
        ///   The path to the test DLL that will be loaded.
        /// </summary>
        private static readonly string TestsPath = @"tests.dll";

        /// <summary>
        ///   The test runner to test.
        /// </summary>
        private RandoopTestRunner runnerUnderTest = null;

        #endregion

        #region Public Methods

        /// <summary>
        /// Tests to see if the ExecuteTests method on the RandoopTestRunner works.
        /// </summary>
        [TestMethod]
        public void ExecuteTestsTest()
        {
            ////ITestSuite mockTests = new MockTestSuite("tests.dll", "tests.dll");
            ////var results = runnerUnderTest.RunTests(mockTests);
        }

        /// <summary>
        /// Initializes properties before each test
        /// </summary>
        [TestInitialize]
        public void PreTestInit()
        {
            this.runnerUnderTest = new RandoopTestRunner();
        }

        #endregion
    }
}