namespace WinBert
{
    using WinBert.Analysis;

    /// <summary>
    /// Defines the basic behavior of a ServiceProvider that handles
    /// </summary>
    public interface IWinBertServiceProvider
    {
        #region Properties

        /// <summary>
        ///   Gets the most recent and valid list of AnalysisResults objects.
        /// </summary>
        AnalysisResults AnalysisResults { get; }

        #endregion
    }
}