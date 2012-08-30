namespace Arktos.WinBert.Instrumentation
{
    using System;

    /// <summary>
    /// Defines the basic behavior of a state machine that logs test cases. Implementations of this interface should be
    ///   geared towards instrumentation--hence the begin and end calls. 
    ///   BMK: Extend this to handle static methods.
    /// </summary>
    public interface ITestCaseLogger
    {
        #region Interface Methods

        /// <summary>
        /// Begins an instrumentation block for a method call.
        /// </summary>
        /// <param name="signature">
        /// The signature of the method called.
        /// </param>
        void BeginMethodCall(string signature);

        /// <summary>
        /// Begins an instrumentation block for a test case.
        /// </summary>
        /// <param name="name">
        /// The optional name of the test case.
        /// </param>
        void BeginTestCase(string name = null);

        /// <summary>
        /// Begins an instrumentation block for a void method call.
        /// </summary>
        /// <param name="signature">
        /// The signature of the method called.
        /// </param>
        void BeginVoidMethodCall(string signature);

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
        void EndMethodCall(object postCallObjectState, object returnValue);

        /// <summary>
        /// Ends an instrumentation block for a test case.
        /// </summary>
        void EndTestCase();

        /// <summary>
        /// Ends an instrumentation block for a void method call.
        /// </summary>
        /// <param name="postCallObjectState">
        /// The state of the object that the method was called on after the method call has executed.
        /// </param>
        void EndVoidMethodCall(object postCallObjectState);

        /// <summary>
        /// Writes an instrumentation block for an Exception. This will likely happen during a method call unless 
        ///   something  goes wrong at the constructor level for the object under test, so it will consequently end 
        ///   the method call block.
        /// </summary>
        /// <param name="exception">
        /// The offending exception.
        /// </param>
        void HandleException(Exception exception);

        #endregion
    }
}