namespace Arktos.WinBert.Differencing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a behavioral difference between a set of test executions.
    /// </summary>
    public class TestExecutionDifference : ITestExecutionDifference
    {
        #region Constructors & Destructors

        public TestExecutionDifference(
            Xml.TestExecution previousExecution,
            Xml.TestExecution currentExecution,
            IEnumerable<IMethodCallDifference> methodDiffs)
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
            this.MethodDifferences = this.FilterMethodDifferences(methodDiffs);
            this.AreDifferences = this.MethodDifferences.Any();
            this.TotalDistance = this.MethodDifferences.Sum(x => x.Distance);
            this.TestName = currentExecution.Name;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public string TestName { get; private set; }

        /// <inheritdoc />
        public IEnumerable<IMethodCallDifference> MethodDifferences { get; private set; }


        public Xml.TestExecution PreviousExecution { get; private set; }

        /// <inheritdoc />
        public Xml.TestExecution CurrentExecution { get; private set; }

        /// <inheritdoc />
        public bool AreDifferences { get; private set; }

        /// <inheritdoc />
        public int? TotalDistance { get; private set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Filters method call differences to only those that have differences.
        /// </summary>
        /// <param name="toFilter">
        /// The list to filter.
        /// </param>
        /// <returns>
        /// A filtered list containing only method call differences that have actual differences.
        /// </returns>
        private IEnumerable<IMethodCallDifference> FilterMethodDifferences(IEnumerable<IMethodCallDifference> toFilter)
        {
            IEnumerable<IMethodCallDifference> filtered;
            if (toFilter == null)
            {
                filtered = Enumerable.Empty<IMethodCallDifference>();
            }
            else
            {
                filtered = toFilter.Where(x => x.AreDifferences);
            }

            return filtered;
        }

        #endregion
    }
}
