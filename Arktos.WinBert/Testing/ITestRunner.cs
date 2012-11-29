namespace Arktos.WinBert.Testing
{
    using AppDomainToolkit;

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
        /// The assembly target containing the protram to execute the tests against.
        /// </param>
        /// <param name="tests">
        /// The assembly containing the tests to execute.
        /// </param>
        /// <returns>
        /// A test run result.
        /// </returns>
        ITestRunResult RunTests(IAssemblyTarget target, IAssemblyTarget tests);
        
        #endregion
    }
}
