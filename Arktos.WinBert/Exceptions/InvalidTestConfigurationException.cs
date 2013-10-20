namespace Arktos.WinBert.Exceptions
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Thrown when a Randoop test doesn't match the expected pattern implemented inside the
    /// test rewriter.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    [Serializable]
    public class InvalidTestConfigurationException : Exception
    {
        #region Constructors & Destructors

        public InvalidTestConfigurationException(string message)
            : base(message)
        {
        }

        public InvalidTestConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion
    }
}
