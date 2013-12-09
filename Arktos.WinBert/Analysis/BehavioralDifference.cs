namespace Arktos.WinBert.Analysis
{
    using Arktos.WinBert.Xml;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BehavioralDifference
    {
        #region Constructors & Destructors

        public BehavioralDifference(
            TestExecution previousExecution,
            TestExecution currentExecution,
            IDictionary<string, TypeDifferenceLookup> diffLookups)
        {
            if (previousExecution == null)
            {
                throw new ArgumentNullException("previousExecution");
            }

            if (currentExecution == null)
            {
                throw new ArgumentNullException("currentExecution");
            }

            if (!previousExecution.Name.Equals(currentExecution.Name, StringComparison.OrdinalIgnoreCase))
            {
                string message = string.Format(
                    "Test execution names do not match! Unable to perform diff! Previous: {0}, Current: {1}",
                    previousExecution.Name,
                    currentExecution.Name);
                throw new ArgumentException(message);
            }

            if (diffLookups == null)
            {
                throw new ArgumentNullException("diffLookupDictionary");
            }

            this.TestName = currentExecution.Name;
            this.PreviousExecution = previousExecution;
            this.CurrentExecution = currentExecution;
            this.MethodDifferences = ComputeMethodDifferences(previousExecution.MethodCalls, currentExecution.MethodCalls, diffLookups);
        }

        #endregion

        #region Properties

        public string TestName { get; private set; }

        public IEnumerable<MethodDifference> MethodDifferences { get; private set; }

        public TestExecution PreviousExecution { get; private set; }

        public TestExecution CurrentExecution { get; private set; }

        public Boolean AreDifferences
        {
            get
            {
                return this.MethodDifferences.Any(x => x.AreDifferences);
            }
        }

        #endregion

        #region Private Methods

        public IEnumerable<MethodDifference> ComputeMethodDifferences(
            IEnumerable<MethodCall> previousCalls,
            IEnumerable<MethodCall> currentCalls,
            IDictionary<string, TypeDifferenceLookup> diffLookups)
        {
            // Join on Id, then create a new method difference from the correlated method executions.
            return previousCalls.Join(
                currentCalls,
                previous => previous.Id,
                current => current.Id,
                (previous, current) =>
                {
                    var lookup = diffLookups[current.PostCallInstance.Type];
                    return new MethodDifference(previous, current, lookup);
                });
        }
        #endregion
    }
}
