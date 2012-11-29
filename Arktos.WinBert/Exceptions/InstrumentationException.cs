namespace Arktos.WinBert.Exceptions
{
    using System;
    
    /// <summary>
    /// Represents an exception thrown when something occurs during the instrumetation pass.
    /// </summary>
    public class InstrumentationException : Exception
    {
        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the InstrumentationException class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public InstrumentationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the InstrumentationException class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="cause">
        /// The inner exception.
        /// </param>
        public InstrumentationException(string message, Exception cause)
            : base(message, cause)
        {
        }

        #endregion
    }
}
