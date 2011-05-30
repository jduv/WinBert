namespace WinBert.Instrumentation
{
    using System;
    using System.IO;

    /// <summary>
    /// A basic test case logging singleton that will handle writing the analysis object model as the instrumented
    ///   test cases run.
    /// </summary>
    public class BertTestCaseLogger : ITestCaseLogger
    {
        #region Constants and Fields

        /// <summary>
        ///   A static, thread-safe instance of the TestCaseLogger class available for other classes to use.
        /// </summary>
        public static readonly BertTestCaseLogger Logger = new BertTestCaseLogger();

        /// <summary>
        ///   Temporary this.output.
        /// </summary>
        private readonly StreamWriter output = null;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Prevents a default instance of the BertTestCaseLogger class from being created.
        /// </summary>
        private BertTestCaseLogger()
        {
            this.output = new StreamWriter("out.txt");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Begins an instrumentation block for a method call.
        /// </summary>
        /// <param name="signature">
        /// The signature of the method called.
        /// </param>
        public void BeginMethodCall(string signature)
        {
            this.output.WriteLine("BeginMethodCall");
            this.output.WriteLine("\t" + signature);
        }

        /// <summary>
        /// Begins an instrumentation block for a test case.
        /// </summary>
        /// <param name="name">
        /// The optional name of the test case.
        /// </param>
        public void BeginTestCase(string name = null)
        {
            this.output.WriteLine("BeginTestCase");
        }

        /// <summary>
        /// Begins an instrumentation block for a void method call.
        /// </summary>
        /// <param name="signature">
        /// The signature of the method called.
        /// </param>
        public void BeginVoidMethodCall(string signature)
        {
            this.output.WriteLine("BeginVoidMethodCall");
            this.output.WriteLine("\t" + signature);
        }

        /// <summary>
        /// Ends an instrumentation block for a method call.
        /// </summary>
        /// <param name="postCallObjectState">
        /// The state of the object that the method was called on after the method call has executed.
        /// </param>
        /// <param name="returnValue">
        /// The return value of the method, if any. If the method signature is void, then the 
        ///   <see cref="EndVoidMethodCall"/> should be used instead.
        /// </param>
        public void EndMethodCall(object postCallObjectState, object returnValue)
        {
            var retVal = string.Format("ReturnValue => {0} {1}", returnValue.GetType().FullName, returnValue.ToString());

            this.output.WriteLine(retVal);
            this.output.Write("EndMethodCall");
        }

        /// <summary>
        /// Ends an instrumentation block for a test case.
        /// </summary>
        public void EndTestCase()
        {
            this.output.WriteLine("EndTestCase");
        }

        /// <summary>
        /// Ends an instrumentation block for a void method call.
        /// </summary>
        /// <param name="postCallObjectState">
        /// The state of the object that the method was called on after the method call has executed.
        /// </param>
        public void EndVoidMethodCall(object postCallObjectState)
        {
            this.output.WriteLine("EndVoidMethodCall");
        }

        /// <summary>
        /// Writes an instrumentation block for an Exception. This will likely happen during a method call unless 
        ///   something goes wrong at the constructor level for the object under test, so it will consequently end the 
        ///   method call block.
        /// </summary>
        /// <param name="exception">
        /// The offending exception.
        /// </param>
        public void HandleException(Exception exception)
        {
            this.output.WriteLine("Exception thrown => " + exception);
        }

        #endregion
    }
}