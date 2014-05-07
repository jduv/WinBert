namespace Arktos.WinBert.Analysis
{
    using Arktos.WinBert.Differencing;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Handles computing differences between test executions.
    /// </summary>
    class TestExecutionDiffer : IDifferenceEngine<Xml.TestExecution, TestExecutionDifference>
    {
        #region Fields & Constants

        private readonly IAssemblyDifference assemblyDiff;
        private readonly IDictionary<string, IDifferenceEngine<Xml.MethodCall, MethodCallDifference>> methodDifferLookup;

        #endregion

        #region Constructors & Destructors

        public TestExecutionDiffer(IAssemblyDifference assemblyDiff)
        {
            if (assemblyDiff == null)
            {
                throw new ArgumentNullException("assemblyDiff");
            }

            this.assemblyDiff = assemblyDiff;
            this.methodDifferLookup = new Dictionary<string, IDifferenceEngine<Xml.MethodCall, MethodCallDifference>>();
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public TestExecutionDifference Diff(Xml.TestExecution oldObject, Xml.TestExecution newObject)
        {
            if (oldObject == null)
            {
                throw new ArgumentNullException("oldObject");
            }

            if (newObject == null)
            {
                throw new ArgumentNullException("newObject");
            }

            // Make sure both tests are named the same.
            if (!oldObject.Name.Equals(newObject.Name, StringComparison.OrdinalIgnoreCase))
            {
                string message = string.Format(
                    "Test execution names do not match! Unable to perform diff! Previous: {0}, Current: {1}",
                    oldObject.Name,
                    newObject.Name);
                throw new ArgumentException(message);
            }

            var methodDiffs = this.ComputeMethodDifferences(oldObject.MethodCalls, newObject.MethodCalls).Where(m => m.Distance != null);
            return new TestExecutionDifference(oldObject, newObject, methodDiffs);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Computes the differences between the two sequences of method calls.
        /// </summary>
        /// <param name="previousCalls">
        /// The old sequence of method calls.
        /// </param>
        /// <param name="currentCalls">
        /// The new sequence of method calls.
        /// </param>
        /// <param name="differ">
        /// The method call differ to use.
        /// </param>
        /// <returns>
        /// A list of method call differences.
        /// </returns>
        public IEnumerable<MethodCallDifference> ComputeMethodDifferences(
            IEnumerable<Xml.MethodCall> previousCalls,
            IEnumerable<Xml.MethodCall> currentCalls)
        {
            // Join on Id, then create a new method difference from the correlated method executions.
            return previousCalls.Join(
                currentCalls,
                previous => previous.Id,
                current => current.Id,
                (previous, current) =>
                {
                    // First, grab a type diff.
                    var typeDiff = this.assemblyDiff[current.PostCallInstance.FullName];

                    // Next, get or create the corresponding method call differ
                    var methodCallDiffer = this.GetOrCreateMethodCallDiffer(typeDiff);

                    // Finally, do the diff
                    return methodCallDiffer.Diff(previous, current);
                });
        }

        /// <summary>
        /// Looks in the local dictionary for a method call differ that supports the target type contained in the
        /// passed type difference. If one doesn't exist, it'll simply new one up for you and return it after placing
        /// it in the local cache for reuse later.
        /// </summary>
        /// <param name="typeDiff">
        /// The type diff to base the method call differ from.
        /// </param>
        /// <returns>
        /// A method call differ capable of handling method calls corresponding to the type information contained inside
        /// the passed type difference.
        /// </returns>
        private IDifferenceEngine<Xml.MethodCall, MethodCallDifference> GetOrCreateMethodCallDiffer(ITypeDifference typeDiff)
        {
            // Ensure that the type diff is not null
            if (typeDiff == null)
            {
                throw new ArgumentNullException("typeDiff");
            }

            IDifferenceEngine<Xml.MethodCall, MethodCallDifference> differ;
            if (this.methodDifferLookup.ContainsKey(typeDiff.FullName))
            {
                differ = this.methodDifferLookup[typeDiff.FullName];
            }
            else
            {
                differ = new MethodCallDiffer(typeDiff);
                this.methodDifferLookup[typeDiff.FullName] = differ;
            }

            return differ;
        }

        #endregion
    }
}
