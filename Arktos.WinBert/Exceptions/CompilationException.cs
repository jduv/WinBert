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
    [Serializable]
    public class CompilationException : Exception
    {
        #region Fields & Constants

        private static readonly string DefaultErrorMessage = @"There was a compliation error!";

        #endregion

        #region Constructors & Destructors

        public CompilationException(string message)
            : base(message)
        {
        }

        public CompilationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        public CompilationException(string message, CompilerResults result)
            : this(message)
        {
            this.Results = result;
        }

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
