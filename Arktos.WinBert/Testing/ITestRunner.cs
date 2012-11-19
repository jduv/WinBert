namespace Arktos.WinBert.Testing
{
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.Environment;

    /// <summary>
    /// Implementations of this interface should be able to execute tests.
    /// </summary>
    public interface ITestRunner
    {
        #region Methods

        /// <summary>
        /// Executes the tests in the target assembly.
        /// </summary>
        /// <param name="target">
        /// The assembly target containing the tests to execute.
        /// </param>
        /// <returns>
        /// A test run result.
        /// </returns>
        TestRunResult RunTests(ILoadedAssemblyTarget target);
        
        #endregion
    }
}
