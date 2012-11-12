namespace Arktos.WinBert.Testing
{
    using Arktos.WinBert.Analysis;

    /// <summary>
    /// Implementations of this interface should be able to execute test suites.
    /// </summary>
    public interface ITestRunner
    {
        /// <summary>
        /// Executes the target test suite and returns an analysis result.
        /// </summary>
        /// <param name="testSuite">
        /// The test suite to execute.
        /// </param>
        /// <returns>
        /// An analysis result.
        /// </returns>
        AnalysisResult RunTests(IRegressionTestSuite testSuite);
    }
}
