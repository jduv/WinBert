namespace Arktos.WinBert.Differencing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a behavioral difference between a set of test executions.
    /// </summary>
    public class TestExecutionDifference : IDifferenceResult
    {
        #region Constructors & Destructors

        public TestExecutionDifference(
            Xml.TestExecution previousExecution,
            Xml.TestExecution currentExecution,
            IEnumerable<MethodCallDifference> methodDiffs)
        {
            if (previousExecution == null)
            {
                throw new ArgumentNullException("oldObject");
            }

            if (currentExecution == null)
            {
                throw new ArgumentNullException("newObject");
            }

            this.PreviousExecution = previousExecution;
            this.CurrentExecution = currentExecution;
            this.MethodDifferences = methodDiffs ?? Enumerable.Empty<MethodCallDifference>();
            this.AreDifferences = this.MethodDifferences.Any(x => x.AreDifferences);
            this.TotalDistance = this.MethodDifferences.Sum(x => x.Distance);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the test. This is the same as grabbing the name off the current test
        /// execution.
        /// </summary>
        public string TestName
        {
            get
            {
                return this.CurrentExecution.Name;
            }
        }

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
        public Boolean AreDifferences { get; private set; }

        /// <summary>
        /// Gets the total distance by summing the distances of each contained method difference. Allows
        /// for sorting test execution differences by maximum distance.
        /// </summary>
        public int? TotalDistance { get; private set; }

        #endregion
    }
}
