namespace Arktos.WinBert.Differencing
{
    using System.Collections.Generic;

    /// <summary>
    /// Contract for a test execution difference.
    /// </summary>
    public interface ITestExecutionDifference : IDifferenceResult
    {
        #region Properties

        /// <summary>
        /// Gets the name of the test. This is the same as grabbing the name off the current test
        /// execution.
        /// </summary>
        string TestName { get; }

        /// <summary>
        /// Gets the current test execution.
        /// </summary>
        Xml.TestExecution CurrentExecution { get; }

        /// <summary>
        /// Gets the previous test execution.
        /// </summary>
        Xml.TestExecution PreviousExecution { get; }

        /// <summary>
        /// Gets a list of method differences, representing changes inside the state of an object upon which
        /// test methods are executed.
        /// </summary>
        IEnumerable<IMethodCallDifference> MethodDifferences { get; }

        /// <summary>
        /// Gets the total distance by summing the distances of each contained method difference. Allows
        /// for sorting test execution differences by maximum distance.
        /// </summary>
        int? TotalDistance { get; }

        #endregion
    }
}
