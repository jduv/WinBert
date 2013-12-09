namespace Arktos.WinBert.Analysis
{
    using Arktos.WinBert.Xml;
    using System;

    public class MethodDifference
    {
        #region Constructors & Destructors

        public MethodDifference(MethodCall previousCall, MethodCall currentCall, TypeDifferenceLookup diffLookup)
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
            if (!diffLookup.TypeName.Equals(currentCall.PostCallInstance.Type, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Error: Type lookup doesn't support post call instance type for the target method!");
            }

            this.Distance = ComputeDistance(currentCall, diffLookup);
            this.ObjectDifference = ComputePostCallObjectDifference(previousCall.PostCallInstance, currentCall.PostCallInstance);
            this.ReturnValueDifference = ComputeReturnValueDifference(previousCall.ReturnValue, currentCall.ReturnValue);
        }

        #endregion

        #region Properties

        public int? Distance { get; private set; }

        public ObjectDifference ObjectDifference { get; private set; }

        public ValueDifference ReturnValueDifference { get; private set; }

        public MethodCall PreviousCall { get; private set; }

        public MethodCall CurrentCall { get; private set; }

        public bool AreDifferences
        {
            get
            {
                return this.ObjectDifference != null || this.ReturnValueDifference != null;
            }
        }

        #endregion

        #region Private Methods

        private static int? ComputeDistance(MethodCall execution, TypeDifferenceLookup lookup)
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
