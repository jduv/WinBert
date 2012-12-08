namespace Arktos.WinBert.Extensions
{
    /// <summary>
    /// Object extension methods.
    /// </summary>
    public static class Objects
    {
        #region Extension Methods

        /// <summary>
        /// Determines if the target object is primitive.
        /// </summary>
        /// <param name="obj">
        /// The object to test.
        /// </param>
        /// <returns>
        /// True if the object is primitive, false otherwise.
        /// </returns>
        public static bool IsPrimitive(this object obj)
        {
            return obj.GetType().IsPrimitive || obj is decimal || obj is string;
        }

        #endregion
    }
}
