namespace Arktos.WinBert.Instrumentation
{
    using System;
    using System.Collections.Generic;
    using Arktos.WinBert.Xml;

    /// <summary>
    /// Dumps tests.
    /// </summary>
    public sealed class TestDumper : ITestDumper
    {
        #region Fields & Constants

        private WinBertAnalysisLog analysisLog = new WinBertAnalysisLog();
        private IList<TestExecution> testExecutions = new List<TestExecution>();
        private TestExecution currentExecution;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDumper"/> class.
        /// </summary>
        public TestDumper()
        {
            this.TestCounter = 0;
            this.MethodCounter = 0;
        }

        #endregion

        #region Properties

        ///<inheritdoc />
        public uint TestCounter { get; private set; }
        
        ///<inheritdoc />
        public uint MethodCounter { get; private set; }
        
        #endregion

        #region Public Methods

        ///<inheritdoc />
        public void StartTest()
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public void EndTest(string path)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public void DumpVoidInstanceMethodCall(object target, string signature)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public void DumpInstanceMethodCall(object target, object returnValue, string signature)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public void AddMethodToDynamicCallGraph(string signature)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
