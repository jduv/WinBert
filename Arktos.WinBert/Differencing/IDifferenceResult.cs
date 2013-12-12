namespace Arktos.WinBert.Differencing
{
    /// <summary>
    /// Defines behavior for an object that describes a difference between two objects.
    /// </summary>
    public interface IDifferenceResult
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether a difference exists.
        /// </summary>
        bool AreDifferences { get; }

        #endregion
    }
}