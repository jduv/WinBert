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

        private IList<TestExecution> testExecutions;
        private IList<MethodCall> methodCalls;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStateRecorder"/> class.
        /// </summary>
        public TestStateRecorder()
        {
            // Interface properties
            this.TestCounter = 0;
            this.MethodCounter = 0;
            this.AnalysisLog = new WinBertAnalysisLog();

            // My specialized properties
            this.testExecutions = new List<TestExecution>();
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
        /// Gets the currently selected test. Mainly exists as a test seam to verify the
        /// state of the dumper. Please do not modify.
        /// </summary>
        public TestExecution CurrentTest { get; private set; }

        /// <summary>
        /// Gets an enumeration of all the logged test executions thus far. Mainly exists
        /// as a test seam to verify the state of the dumper.
        /// </summary>
        public IEnumerable<TestExecution> TestExeuctions
        {
            get
            {
                return this.testExecutions;
            }
        }

        #endregion

        #region Public Methods

        ///<inheritdoc />
        public void StartTest()
        {
            // Create a new execution.
            this.CurrentTest = new TestExecution() { Id = this.MethodCounter };

            // Reset method calls list.
            this.methodCalls = new List<MethodCall>();
        }

        ///<inheritdoc />
        public void EndTest()
        {
            this.ValidateCurrentTestState();

            // Use field, keeps list operations private.
            this.testExecutions.Add(this.CurrentTest);
            this.AnalysisLog.TestExecutions = this.testExecutions.ToArray();
            this.CurrentTest = null;
            this.TestCounter++;
        }

        ///<inheritdoc />
        public void RecordVoidInstanceMethodCall(object target, string signature)
        {
            this.ValidateCurrentTestState();

            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public void RecordInstanceMethodCall(object target, object returnValue, string signature)
        {
            this.ValidateCurrentTestState();

            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public void AddMethodToDynamicCallGraph(string signature)
        {
            this.ValidateCurrentTestState();

            throw new NotImplementedException();
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
