﻿namespace Arktos.WinBert.Exceptions
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

        public InvalidConfigurationException(string message)
            : base(message)
        {
        }

        public InvalidConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion
    }
}