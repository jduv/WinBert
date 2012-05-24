namespace WinBert.Differencing
{
    /// <summary>
    /// An instance of a difference engine will be able to take two compilation units, in most cases
    ///   a type or an assembly,  and enumerate the differences between the two. Implementations of this
    ///   interface allow for a diverse differencing mechanism specialized to the tasks needed to be completed.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object that the implementing engine will be able to difference
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The resulting type.
    /// </typeparam>
    public interface IDifferenceEngine<T, TResult>
        where TResult : IDifferenceResult<T>
    {
        #region Interface Methods

        /// <summary>
        /// This method should examine the passed in parameters, determine if they are different, and return
        ///   a value indicating the state of the operation. This could be as simple as a Boolean value (yes it's
        ///   different or no it's not) to a complex structure organizing all the differences.
        /// </summary>
        /// <param name="oldObject">
        /// The first object to diff.
        /// </param>
        /// <param name="newObject">
        /// The second object to diff.
        /// </param>
        /// <returns>
        /// A difference result.
        /// </returns>
        TResult Diff(T oldObject, T newObject);

        #endregion
    }
}