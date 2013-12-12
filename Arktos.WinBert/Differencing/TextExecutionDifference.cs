namespace Arktos.WinBert.Differencing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a behavioral difference between a set of method executions. The root of this class is the
    /// test execution set that spawned it, the set of which methods inside each that will be used to create 
    /// method differences. These method differences are what we are mainly interested in for the Bert analysis.
    /// </summary>
    public class TestExecutionDifference
    {
        #region Constructors & Destructors

        public TestExecutionDifference(
            Xml.TestExecution previousExecution,
            Xml.TestExecution currentExecution,
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

        /// <summary>
        /// Gets the name of the test.
        /// </summary>
        public string TestName { get; private set; }

        /// <summary>
        /// Gets a list of method differences, representing changes inside the state of an object upon which
        /// test methods are executed.
        /// </summary>
        public IEnumerable<MethodCallDifference> MethodDifferences { get; private set; }

        /// <summary>
        /// Gets the previous test execution.
        /// </summary>
        public Xml.TestExecution PreviousExecution { get; private set; }

        /// <summary>
        /// Gets the current test execution.
        /// </summary>
        public Xml.TestExecution CurrentExecution { get; private set; }

        /// <summary>
        /// Gets a value indicating whether there any differences in the set of all methods 
        /// analyzed in this instance.
        /// </summary>
        public Boolean AreDifferences
        {
            get
            {
                return this.MethodDifferences.Any(x => x.AreDifferences);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Computes the method difference between the two sets of method calls.
        /// </summary>
        /// <param name="previousCalls">
        /// The previous set of method calls.
        /// </param>
        /// <param name="currentCalls">
        /// The current set of method calls.
        /// </param>
        /// <param name="diffLookups">
        /// A dictionary of type difference lookups.
        /// </param>
        /// <returns>
        /// A list of method differences.
        /// </returns>
        public static IEnumerable<MethodCallDifference> ComputeMethodDifferences(
            IEnumerable<Xml.MethodCall> previousCalls,
            IEnumerable<Xml.MethodCall> currentCalls,
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
                    return new MethodCallDifference(previous, current, lookup);
                });
        }
        #endregion
    }
}
