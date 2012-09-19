namespace Arktos.WinBert.Exceptions
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// This exception should be thrown when some piece of configuration is in an invalid state.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class InvalidConfigurationException : Exception
    {
        #region Constructors & Destructors

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