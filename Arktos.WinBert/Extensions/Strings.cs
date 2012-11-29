namespace Arktos.WinBert.Extensions
{
    /// <summary>
    /// Contains string extension methods and utilities.
    /// </summary>
    public static class Strings
    {
        #region Public Methods

        /// <summary>
        /// This allows for a nice pretty concatenation of strings using the ?? operator. If the passed in string
        /// is null or empty, then this method will return null.
        /// </summary>
        /// <param name="value">
        /// This value to check.
        /// </param>
        /// <returns>
        /// Null if the string is null or empty, the string otherwise.
        /// </returns>
        public static string IfEmptyThenNull(string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }

        /// <summary>
        /// Returns the string "null" if the target string is null.
        /// </summary>
        /// <param name="value">
        /// The value to check.
        /// </param>
        /// <returns>
        /// The string "null" if the value is null, else the value.
        /// </returns>
        public static string PrettyPrintNull(string value)
        {
            return (value == null) ? "null" : value;
        }

        /// <summary>
        /// Returns the string "null" if the target string is null or empty.
        /// </summary>
        /// <param name="value">
        /// The value to check.
        /// </param>
        /// <returns>
        /// The string "null" if the value is null or empty, else the value.
        /// </returns>
        public static string PrettyPrintNullOrEmpty(string value)
        {
            return string.IsNullOrEmpty(value) ? "null" : value;
        }

        #endregion
    }
}
