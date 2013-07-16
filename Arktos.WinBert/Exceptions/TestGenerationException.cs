namespace Arktos.WinBert.Exceptions
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Throw this exception when an error or issue occurs during test generation.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    [Serializable]
    public class TestGenerationException : Exception
    {
        #region Fields & Constants

        private static readonly string DefaultErrorMessage = "There was an error while generating the tests!";

        #endregion

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
        /// <summary>
        /// Initializes a new instance of the TestGenerationException class.
        /// </summary>
        /// <param name="cause">
        /// The inner exception.
        /// </param>
        public TestGenerationException(Exception cause)
            : base(DefaultErrorMessage, cause)
        {
        }

        #endregion
    }
}
