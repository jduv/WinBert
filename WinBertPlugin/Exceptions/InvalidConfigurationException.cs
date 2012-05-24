namespace WinBert.Exceptions
{
    using System;

    /// <summary>
    /// This exception should be thrown when some piece of configuration is in an invalid state.
    /// </summary>
    public sealed class InvalidConfigurationException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the InvalidConfigurationException class.
        /// </summary>
        /// <param name="message">
        /// The message for the exception.
        /// </param>
        public InvalidConfigurationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidConfigurationException class.
        /// </summary>
        /// <param name="message">
        /// The message for the exception.
        /// </param>
        /// <param name="innerException">
        /// An inner exception.
        /// </param>
        public InvalidConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion
    }
}