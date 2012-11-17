namespace Arktos.WinBert.Testing
{
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.Xml;

    /// <summary>
    /// Defines the base behavior for all regression test managers.
    /// </summary>
    public interface IRegressionTestManager
    {
        #region Methods
        
        /// <summary>
        /// Builds and executes all tests for the two target builds.
        /// </summary>
        /// <param name="current">
        /// The current build.
        /// </param>
        /// <param name="previous">
        /// The previous build.
        /// </param>
        /// <returns>
        /// An analysis result.
        /// </returns>
        AnalysisResult BuildAndExecuteTests(Build current, Build previous);

        #endregion
    }
}
