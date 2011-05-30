namespace WinBert.Testing
{
    /// <summary>
    /// Implementations of this interface should be able to run a set of tests passed into the
    ///   object in the form of a TestSuite object.
    /// </summary>
    public interface ITestSuiteRunner
    {
        #region Interface Methods

        /// <summary>
        /// Runs all tests in the target test type.
        /// </summary>
        /// <param name="tests">
        /// The test suite of which to run the tests from.
        /// </param>
        /// <returns>
        /// A TestRunResult object indicating the results of the test run.
        /// </returns>
        TestSuiteRunResult RunTests(ITestSuite tests);

        #endregion
    }
}