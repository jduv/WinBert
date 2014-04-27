namespace Arktos.WinBert.Differencing
{
    using System;

    /// <summary>
    /// Computes diffs on method calls.
    /// </summary>
    class MethodCallDiffer : IDifferenceEngine<Xml.MethodCall, MethodCallDifference>
    {
        #region Fields & Constants

        private readonly ITypeDifference typeDiff;
        private readonly IAnalysisLogObjectDiffer objectDiffer;

        #endregion

        #region Constructors & Destructors

        public MethodCallDiffer(ITypeDifference typeDiff)
            : this(typeDiff, new AnalysisLogObjectDiffer())
        {
        }

        public MethodCallDiffer(
            ITypeDifference typeDiff,
            IAnalysisLogObjectDiffer objectDiffer)
        {
            if (typeDiff == null)
            {
                throw new ArgumentNullException("typeDiff");
            }

            if (objectDiffer == null)
            {
                throw new ArgumentNullException("objectDiffer");
            }

            this.typeDiff = typeDiff;
            this.objectDiffer = objectDiffer;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public MethodCallDifference Diff(Xml.MethodCall oldObject, Xml.MethodCall newObject)
        {
            if (oldObject == null)
            {
                throw new ArgumentNullException("oldObject");
            }

            if (newObject == null)
            {
                throw new ArgumentNullException("newObject");
            }

            if (!oldObject.Signature.Equals(newObject.Signature, StringComparison.OrdinalIgnoreCase))
            {
                string message = string.Format(
                    "Method signatures do not match! Unable to perform diff! Previous: {0}, Current: {1}",
                    oldObject.Signature,
                    newObject.Signature);
                throw new ArgumentException(message);
            }

            // make sure we can actually handle the method calls based on type.
            if (!this.typeDiff.FullName.Equals(newObject.PostCallInstance.FullName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Error: Type lookup doesn't support post call instance type for the target method!");
            }

            return new MethodCallDifference(
                oldObject,
                newObject,
                this.ComputeDistance(newObject),
                this.objectDiffer.DiffObjects(oldObject.PostCallInstance, newObject.PostCallInstance),
                this.objectDiffer.DiffReturnValues(oldObject.ReturnValue, newObject.ReturnValue));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Computes the distance between the target method call and any method that has changed in the
        /// type diff tree. If the current method has changed, then this value will always be zero. If this
        /// method never calls a changed method, we'll return null.
        /// </summary>
        /// <param name="call">
        /// The call to analyze.
        /// </param>
        /// <returns>
        /// The distance value from the target method call to a method in the dynamic call graph that has changed.
        /// If no changed method is found in the call stack, this will return null.
        /// </returns>
        private int? ComputeDistance(Xml.MethodCall call)
        {
            if (this.typeDiff.Contains(call.Signature))
            {
                return 0;
            }
            else
            {
                int index;
                bool found = false;
                for (index = 0; index < call.DynamicCallGraph.Count; index++)
                {
                    if (this.typeDiff.Contains(call.DynamicCallGraph[index].Signature))
                    {
                        found = true;
                        break;
                    }
                }

                return found ? index : (int?)null;
            }
        }

        #endregion
    }
}
