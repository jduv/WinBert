namespace Arktos.WinBert.Differencing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
            IEnumerable<IAnalysisLogDiff> postCallDiffs,
            IEnumerable<IAnalysisLogDiff> returnValueDiffs = null)
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

            if (postCallDiffs == null)
            {
                throw new ArgumentNullException("postCallDiff");
            }

            this.PreviousCall = previousCall;
            this.CurrentCall = currentCall;
            this.Distance = distance;
            this.PostCallObjectDifferences = postCallDiffs;
            this.ReturnValueDifferences = returnValueDiffs;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the distance value of this method call to one that has changed in a dynamic call graph produced
        /// by this method call.
        /// </summary>
        public int? Distance { get; private set; }

        /// <summary>
        /// Gets the difference between the post-call objects in the old and new test executions.
        /// </summary>
        public IEnumerable<IAnalysisLogDiff> PostCallObjectDifferences { get; private set; }

        /// <summary>
        /// Gets the difference between the return values. This could be a primitive difference or
        /// an object difference depending on what's returned by the method call.
        /// </summary>
        public IEnumerable<IAnalysisLogDiff> ReturnValueDifferences { get; private set; }

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
        public bool AreDifferences
        {
            get
            {
                return this.PostCallObjectDifferences.Any() || (this.ReturnValueDifferences != null && this.ReturnValueDifferences.Any());
            }
        }

        #endregion
    }
}
