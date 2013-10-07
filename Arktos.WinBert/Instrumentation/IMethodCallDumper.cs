namespace Arktos.WinBert.Instrumentation
{
    using System;

    /// <summary>
    /// Interface for objects that can dump method call information.
    /// </summary>
    public interface IMethodCallDumper
    {
        #region Methods

        /// <summary>
        /// Logs a void instance method call.
        /// </summary>
        /// <param name="id">
        /// The id to assign to the call.
        /// </param>
        /// <param name="target">
        /// The post call instance to log. This should be the object that the instance method was executed
        /// upon.
        /// </param>
        /// <param name="signature">
        /// The signature of the executed method.
        /// </param>
        /// <returns>
        /// An Xml.MethodCall instance.
        /// </returns>
        Xml.MethodCall DumpInstanceMethod(uint id, object target, object returnValue, string signature);

        /// <summary>
        /// Dumps an instance method call with a return value.
        /// </summary>
        /// <param name="id">
        /// The id to assign to the call.
        /// </param>
        /// <param name="target">
        /// The post call instance to log. This should be the object that the instance method was executed
        /// upon.
        /// </param>
        /// <param name="returnValue">
        /// The return value of the method.
        /// </param>
        /// <param name="signature">
        /// The signature of the executed method.
        /// </param>
        /// <returns>
        /// An Xml.MethodCall instance.
        /// </returns>
        Xml.MethodCall DumpVoidInstanceMethod(uint id, object target, string signature);

        #endregion
    }
}
