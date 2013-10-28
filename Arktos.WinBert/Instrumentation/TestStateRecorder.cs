namespace Arktos.WinBert.Instrumentation
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Arktos.WinBert.Xml;

    /// <summary>
    /// Dumps tests. There's a bit of state saved in this little class, and things could possibly
    /// get out of sync if certain methods are called in the wrong order. This implementation 
    /// should be fairly solid, however, with sentinals in place where possible to keep the state
    /// machine sane.
    /// </summary>
    public sealed class TestStateRecorder : ITestStateRecorder
    {
        #region Fields & Constants

        private readonly IMethodCallDumper methodDumper;
        private List<CallGraphNode> currentCallGraph = new List<CallGraphNode>();

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStateRecorder"/> class.
        /// </summary>
        public TestStateRecorder()
            : this(new MethodCallDumper())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStateRecorder"/> class. This constructor is more or less
        /// a seam for testing.
        /// </summary>
        /// <param name="methodDumper">
        /// The method call dumper to use when logging method call state.
        /// </param>
        public TestStateRecorder(IMethodCallDumper methodDumper)
        {
            // Interface properties
            this.TestCounter = 0;
            this.MethodCounter = 0;
            this.AnalysisLog = new WinBertAnalysisLog() { TestExecutions = new List<TestExecution>() };
            this.methodDumper = methodDumper;
        }

        #endregion

        #region Properties

        ///<inheritdoc />
        public uint TestCounter { get; private set; }

        ///<inheritdoc />
        public uint MethodCounter { get; private set; }

        /// <inheritdoc />
        public WinBertAnalysisLog AnalysisLog { get; private set; }

        /// <summary>
        /// Gets the currently selected test. Exists as a test seam to verify the
        /// state of the recorder.
        /// </summary>
        public TestExecution CurrentTest { get; private set; }

        /// <summary>
        /// Gets the currently selected method call. Exists as a test seam to verify the state of
        /// the recorder.
        /// </summary>
        public MethodCall CurrentMethodCall { get; private set; }

        /// <summary>
        /// Gets the currently selected value of the call graph counter.
        /// </summary>
        public uint CallGraphCounter { get; private set; }

        #endregion

        #region Public Methods

        ///<inheritdoc />
        public void StartTest()
        {
            // Create a new execution.
            this.CurrentTest = new TestExecution() { Id = this.TestCounter, MethodCalls = new List<MethodCall>() };
        }

        /// <inheritdoc />
        public void StartTest(string testName)
        {
            this.CurrentTest = new TestExecution() { Id = this.TestCounter, MethodCalls = new List<MethodCall>(), Name = testName };
        }

        ///<inheritdoc />
        public void EndTest()
        {
            this.ValidateCurrentTestState();

            // Use field, keeps list operations private.
            this.AnalysisLog.TestExecutions.Add(this.CurrentTest);
            this.CurrentTest = null;
            this.CurrentMethodCall = null;
            this.TestCounter++;
            this.MethodCounter = 0; // Reset method counter.
        }

        ///<inheritdoc />
        public void RecordVoidInstanceMethodCall(object target, string signature)
        {
            this.ValidateCurrentTestState();
            // Add method and increment method counter
            var dumpedMethod = this.methodDumper.DumpVoidInstanceMethod(this.MethodCounter++, target, signature);
            dumpedMethod.DynamicCallGraph = this.currentCallGraph;
            this.CurrentMethodCall = dumpedMethod;
            this.CurrentTest.MethodCalls.Add(dumpedMethod);
            this.CallGraphCounter = 0; // Reset the call graph counter.
            this.currentCallGraph = new List<CallGraphNode>(); // Reset the internal cg node list.
        }

        ///<inheritdoc />
        public void RecordInstanceMethodCall(object target, object returnValue, string signature)
        {
            this.ValidateCurrentTestState();
            // Add method and increment method counter
            var dumpedMethod = this.methodDumper.DumpInstanceMethod(this.MethodCounter++, target, returnValue, signature);
            dumpedMethod.DynamicCallGraph = this.currentCallGraph;
            this.CurrentMethodCall = dumpedMethod;
            this.CurrentTest.MethodCalls.Add(dumpedMethod);
            this.CallGraphCounter = 0; // Reset the call graph counter.
            this.currentCallGraph = new List<CallGraphNode>(); // Reset the internal cg node list.
        }

        ///<inheritdoc />
        public void AddMethodToDynamicCallGraph(string signature)
        {
            this.ValidateCurrentTestState();
            // Add node and increment call graph counter.
            this.currentCallGraph.Add(new Xml.CallGraphNode() { SequenceNumber = this.CallGraphCounter++, Signature = signature });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Simply throws an exception if the current test is null. This is a pretty common check, thereby
        /// the method.
        /// </summary>
        private void ValidateCurrentTestState()
        {
            if (this.CurrentTest == null)
            {
                var msg = "Invalid state: current test is null and must not be! " +
                    "Ensure that StartTest has already been called.";
                throw new InvalidOperationException(msg);
            }
        }

        #endregion
    }
}
