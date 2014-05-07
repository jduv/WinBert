namespace Arktos.WinBert.Differencing
{

    /// <summary>
    /// Defines the method call difference contract.
    /// </summary>
    public interface IMethodCallDifference : IDifferenceResult
    {
        #region Properties

        /// <summary>
        /// Gets the current method call.
        /// </summary>
        Xml.MethodCall CurrentCall { get; }

        /// <summary>
        /// Gets the previous method call.
        /// </summary>
        Xml.MethodCall PreviousCall { get; }

        /// <summary>
        /// Gets the distance value of this method call to one that has changed in a dynamic call graph produced
        /// by this method call.
        /// </summary>
        int? Distance { get; }

        /// <summary>
        /// Gets the differences between the post-call objects in the old and new test executions.
        /// </summary>
        ObjectDifference PostCallObjectDifferences { get; }

        /// <summary>
        /// Gets the difference between the return values. This could be a primitive difference or
        /// an object difference depending on what's returned by the method call.
        /// </summary>
        ReturnValueDifference ReturnValueDifference { get; }

        /// <summary>
        /// Gets the method call's signature.
        /// </summary>
        string Signature { get; }

        #endregion
    }
}
