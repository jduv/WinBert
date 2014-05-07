namespace Arktos.WinBert.Analysis
{
    using Arktos.WinBert.Differencing;
    using System;

    /// <summary>
    /// Represents and enumerates the differences between two method calls from the XML namespace.
    /// This is a light-weight object used only for viewing differences. The actual computation is
    /// in a differ class.
    /// </summary>
    public class MethodCallDifference : IDifferenceResult
    {
        #region Constructors & Destructors

        public MethodCallDifference(
            Xml.MethodCall previousCall,
            Xml.MethodCall currentCall,
            int? distance,
            ObjectDifference postCallDiff,
            ReturnValueDifference returnValueDiff)
        {
            if (previousCall == null)
            {
                throw new ArgumentNullException("previousCall");
            }

            if (currentCall == null)
            {
                throw new ArgumentNullException("currentCall");
            }

            if (distance < 0)
            {
                throw new ArgumentException("Distance must be positive!");
            }

            if (postCallDiff == null)
            {
                throw new ArgumentNullException("postCallDiff");
            }

            if (returnValueDiff == null)
            {
                throw new ArgumentNullException("returnValueDiff");
            }

            this.PreviousCall = previousCall;
            this.CurrentCall = currentCall;
            this.Distance = distance;
            this.PostCallObjectDifferences = postCallDiff;
            this.ReturnValueDifference = returnValueDiff;
            this.AreDifferences = this.PostCallObjectDifferences.AreDifferences || this.ReturnValueDifference.AreDifferences;
            this.Signature = currentCall.Signature;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the method call's signature.
        /// </summary>
        public string Signature { get; private set; }

        /// <summary>
        /// Gets the distance value of this method call to one that has changed in a dynamic call graph produced
        /// by this method call.
        /// </summary>
        public int? Distance { get; private set; }

        /// <summary>
        /// Gets the differences between the post-call objects in the old and new test executions.
        /// </summary>
        public ObjectDifference PostCallObjectDifferences { get; private set; }

        /// <summary>
        /// Gets the difference between the return values. This could be a primitive difference or
        /// an object difference depending on what's returned by the method call.
        /// </summary>
        public ReturnValueDifference ReturnValueDifference { get; private set; }

        /// <summary>
        /// Gets the previous method call.
        /// </summary>
        public Xml.MethodCall PreviousCall { get; private set; }

        /// <summary>
        /// Gets the current method call.
        /// </summary>
        public Xml.MethodCall CurrentCall { get; private set; }

        /// <summary>
        /// Gets a value indicating whether there are some discreet differences between the two method
        /// calls.
        /// </summary>
        public bool AreDifferences { get; private set; }

        #endregion
    }
}
