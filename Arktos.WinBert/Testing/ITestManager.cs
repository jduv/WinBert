namespace Arktos.WinBert.Testing
{
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.Xml;

    /// <summary>
    /// Defines behavior for an implementation responsible for performing WinBert analysis on
    /// two builds.
    /// </summary>
    public interface ITestManager
    {
        #region Methods

        /// <summary>
        /// Performs the appropriate actions on the target builds in order to perform a BERT regression
        /// test analysis on the two assemlies represented by the build targets.
        /// </summary>
        /// <param name="previous">
        /// The previous build.
        /// </param>
        /// <param name="current">
        /// The current build.
        /// </param>
        /// <returns>
        /// An AnalysisResult.
        /// </returns>
        AnalysisResult BuildAndExecuteTests(Build previous, Build current);

        #endregion
    }
}
