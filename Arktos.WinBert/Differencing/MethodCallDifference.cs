namespace Arktos.WinBert.Differencing
{
    using System;

    /// <summary>
    /// Represents and enumerates the differences between two method calls from the XML namespace.
    /// This instance will compute the difference between post-call states of the object upon which the
    /// target method is invoked and report back a list of those differences.
    /// </summary>
    public class MethodCallDifference
    {
        #region Constructors & Destructors

        public MethodCallDifference(Xml.MethodCall previousCall, Xml.MethodCall currentCall, ITypeDifference diffLookup)
        {
            if (previousCall == null)
            {
                throw new ArgumentNullException("previousCall");
            }

            if (currentCall == null)
            {
                throw new ArgumentNullException("currentCall");
            }

            if (!previousCall.Signature.Equals(currentCall.Signature, StringComparison.OrdinalIgnoreCase))
            {
                string message = string.Format(
                    "Method signatures do not match! Unable to perform diff! Previous: {0}, Current: {1}",
                    previousCall.Signature,
                    currentCall.Signature);
                throw new ArgumentException(message);
            }

            if (diffLookup == null)
            {
                throw new ArgumentNullException("diffLookup");
            }

            // Ensure we have been passed a valid lookup.
            if (!diffLookup.FullName.Equals(currentCall.PostCallInstance.Type, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Error: Type lookup doesn't support post call instance type for the target method!");
            }

            this.Distance = ComputeDistance(currentCall, diffLookup);
            this.ObjectDifference = ComputePostCallObjectDifference(previousCall.PostCallInstance, currentCall.PostCallInstance);
            this.ReturnValueDifference = ComputeReturnValueDifference(previousCall.ReturnValue, currentCall.ReturnValue);
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
        public ObjectDifference ObjectDifference { get; private set; }

        /// <summary>
        /// Gets the difference between the return values. This could be a primitive difference or
        /// an object difference depending on what's returned by the method call.
        /// </summary>
        public ValueDifference ReturnValueDifference { get; private set; }

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
                return this.ObjectDifference != null || this.ReturnValueDifference != null;
            }
        }

        #endregion

        #region Private Methods

        private static int? ComputeDistance(Xml.MethodCall execution, ITypeDifference lookup)
        {
            if (lookup.Contains(execution.Signature))
            {
                return 0;
            }
            else
            {
                int index;
                bool found = false;
                for (index = 0; index < execution.DynamicCallGraph.Count; index++)
                {
                    if (lookup.Contains(execution.DynamicCallGraph[index].Signature))
                    {
                        found = true;
                        break;
                    }
                }

                return found ? index : (int?)null;
            }
        }

        private static ObjectDifference ComputePostCallObjectDifference(Xml.Object previousObject, Xml.Object currentObject)
        {
            // FIXME
            return null;
        }


        private static ValueDifference ComputeReturnValueDifference(Xml.Value previousValue, Xml.Value currentValue)
        {
            ValueDifference returnValueDiff;
            if (previousValue == null && currentValue == null)
            {
                returnValueDiff = null;
            }
            else
            {
                // FIXME
                returnValueDiff = null;
            }

            return returnValueDiff;
        }

        #endregion
    }
}
