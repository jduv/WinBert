namespace WinBert.Differencing
{
    /// <summary>
    /// Implementations of this interface should allow for a concrete representation of a 
    ///   difference between two objects of type T.
    /// </summary>
    /// <typeparam name="T">
    /// The type of objects this implementation is able to difference.
    /// </typeparam>
    public interface IDifferenceResult<T>
    {
        #region Properties

        /// <summary>
        ///   Gets a value indicating whether a difference exists.
        /// </summary>
        bool DifferenceResult { get; }

        /// <summary>
        ///   Gets another version of the differenced arguments.
        /// </summary>
        T NewObject { get; }

        /// <summary>
        ///   Gets one version of the differenced arguments.
        /// </summary>
        T OldObject { get; }

        #endregion
    }
}