namespace Arktos.WinBert.Differencing
{
    using System;

    /// <summary>
    /// Represents and enumerates the differences between two method calls from the XML namespace.
    /// This is a light-weight object used only for viewing differences. The actual computation is
    /// in a differ class.
    /// </summary>
    public class MethodCallDifference : IMethodCallDifference
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

        /// <inheritdoc />
        public string Signature { get; private set; }

        /// <inheritdoc />
        public int? Distance { get; private set; }

        /// <inheritdoc />
        public ObjectDifference PostCallObjectDifferences { get; private set; }

        /// <inheritdoc />
        public ReturnValueDifference ReturnValueDifference { get; private set; }

        /// <inheritdoc />
        public Xml.MethodCall PreviousCall { get; private set; }

        /// <inheritdoc />
        public Xml.MethodCall CurrentCall { get; private set; }

        /// <inheritdoc />
        public bool AreDifferences { get; private set; }

        #endregion
    }
}
