namespace Arktos.WinBert.Exceptions
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Thrown whenever a compiler implementation fails to compile some piece
    /// of source code or a comparable error occurs.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class CompilationException : Exception
    {
        #region Fields & Constants

        /// <summary>
        /// The default error message string for the exception.
        /// </summary>
        private static readonly string DefaultErrorMessage = @"There was a compliation error!";

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationException"/> class.
        /// </summary>
        /// <param name="message">
        /// A message describing the exception.
        /// </param>
        public CompilationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationException"/> class.
        /// </summary>
        /// <param name="message">
        /// A message describing the exception.
        /// </param>
        /// <param name="innerException">
        /// An inner exception.
        /// </param>
        public CompilationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationException"/> class.
        /// </summary>
        /// <param name="message">
        /// A message describing the exception.
        /// </param>
        /// <param name="result">
        /// The compiler result that caused this exception.
        /// </param>
        public CompilationException(string message, CompilerResults result)
            : this(message)
        {
            this.Results = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationException"/> class.
        /// </summary>
        public CompilationException(CompilerResults result)
            : this(DefaultErrorMessage, result)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the compiler results.
        /// </summary>
        public CompilerResults Results
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets an enumeration of the errors contained in the compiler result
        /// that caused this exception to be thrown.
        /// </summary>
        public IEnumerable<CompilerError> CompilerErrors
        {
            get
            {
                if (this.Results != null)
                {
                    foreach (CompilerError error in this.Results.Errors)
                    {
                        yield return error;
                    }
                }
            }
        }

        #endregion
    }
}
