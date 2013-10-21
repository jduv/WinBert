namespace Arktos.WinBert.Exceptions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

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

        public TestGenerationException()
        {
        }

        public TestGenerationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public TestGenerationException(string message)
            : base(message)
        {
        }

        public TestGenerationException(string message, Exception cause)
            : base(message, cause)
        {
        }

        public TestGenerationException(Exception cause)
            : base(DefaultErrorMessage, cause)
        {
        }

        #endregion
    }
}
