namespace Arktos.WinBert.Instrumentation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Arktos.WinBert.Xml;

    /// <summary>
    /// Implementations of this interface should be able to dump information about tests during
    /// execution.  Dynamic call graphs
    /// </summary>
    public interface ITestStateRecorder
    {
        #region Properties

        /// <summary>
        /// Gets the current method counter.
        /// </summary>
        uint MethodCounter { get; }

        /// <summary>
        /// Gets the current test counter.
        /// </summary>
        uint TestCounter { get;  }

        /// <summary>
        /// Gets the analysis log that was recorded.
        /// </summary>
        WinBertAnalysisLog AnalysisLog { get; }

        #endregion

        /// <summary>
        /// Starts a test case. This method should set up all the required state for a test case to execute in-
        /// context. This means that a call to anything other than <see cref="EndTest"/> should log any data
        /// captured to the current test.
        /// </summary>
        void StartTest();

        /// <summary>
        /// Starts a test case, but optionally allows a string identifier name.
        /// </summary>
        /// <param name="testName">
        /// The name to assign to the test.
        /// </param>
        void StartTest(string testName);

        /// <summary>
        /// Ends a test case, setting the state of the dumper up for the next call to <see cref="StartTest"/>.
        /// </summary>
        void EndTest();

        /// <summary>
        /// Dumps a void instance method call inthe current context.
        /// </summary>
        /// <param name="target">
        /// The target object upon which the method was invoked.
        /// </param>
        /// <param name="signature">
        /// The signature of the method that was executed.
        /// </param>
        void RecordVoidInstanceMethodCall(object target, string signature);

        /// <summary>
        /// Dumps a void instance method call inthe current context.
        /// </summary>
        /// <param name="target">
        /// The target object upon which the method was invoked.
        /// </param>
        /// <param name="returnValue">
        /// The return value of the target method.
        /// </param>
        /// <param name="signature">
        /// The signature of the method that was executed.
        /// </param>
        void RecordInstanceMethodCall(object target, object returnValue, string signature);

        /// <summary>
        /// Adds the target signature to the dynamic call graph of the executing context. This method can be
        /// called after StartTest and before any recording calls.
        /// </summary>
        /// <param name="signature">
        /// The signature of the method that was executed.
        /// </param>
        void AddMethodToDynamicCallGraph(string signature);
    }
}
