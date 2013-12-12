namespace Arktos.WinBert.Differencing
{
    /// <summary>
    /// Very simple interface for displaying new/old values for a property or
    /// field difference in an anlysis log. This interface isn't very sophisticated,
    /// it uses strings to represent it's values instead of stricterr data types. However,
    /// this is the most flexiby way to represent a difference in an analysis log object
    /// graph since most type information is erased in serialization.
    /// </summary>
    public interface IAnalysisLogDiff
    {
        #region Properties

        /// <summary>
        /// Gets the path to the difference in the object graph.
        /// </summary>
        IMemberPath Path { get; }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        string OldValue { get; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        string NewValue { get; }

        #endregion
    }
}
