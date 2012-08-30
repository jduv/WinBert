namespace Arktos.WinBert.Testing
{
    using Arktos.WinBert.Differencing;

    /// <summary>
    /// Defines the behavior of classes that generate TestSuites.
    /// </summary>
    public interface IRegressionTestSuiteGenerator
    {
        #region Interface Methods

        /// <summary>
        /// Generates a TestSuite given an assembly diff.
        /// </summary>
        /// <param name="diff">
        /// A static difference context indicating if the object and it's comparator
        ///   (stored inside the difference object) are different.
        /// </param>
        /// <returns>
        /// A meaningful set of tests, compiled into a TestSuite
        /// </returns>
        IRegressionTestSuite GetCompiledTests(AssemblyDifferenceResult diff);

        #endregion
    }
}