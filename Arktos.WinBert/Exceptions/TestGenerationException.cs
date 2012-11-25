namespace Arktos.WinBert.Exceptions
{
    using System;

    /// <summary>
    /// Throw this exception when an error or issue occurs during test generation.
    /// </summary>
    public class TestGenerationException : Exception
    {
        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the TestGenerationException class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public TestGenerationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TestGenerationException class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="cause">
        /// The inner exception.
        /// </param>
        public TestGenerationException(string message, Exception cause)
            : base(message, cause)
        {
        }

        #endregion
    }
}
