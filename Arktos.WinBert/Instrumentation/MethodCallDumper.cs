namespace Arktos.WinBert.Instrumentation
{
    using System;
    using Arktos.WinBert.Extensions;

    /// <summary>
    /// Logs method calls of various types. In essence here we have factory methods for Xml.MethodCall
    /// objects.
    /// </summary>
    public class MethodCallDumper
    {
        #region Constants & Fields

        private static readonly ObjectDumper objDumper;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static MethodCallDumper()
        {
            objDumper = new ObjectDumper();
        }

        #endregion

        #region Public Methods

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
        public Xml.MethodCall DumpVoidInstanceMethod(uint id, object target, string signature)
        {
            return new Xml.MethodCall()
            {
                Id = id,
                Signature = signature,
                Type = Xml.MethodCallType.Instance,
                PostCallInstance = objDumper.DumpObject(target)
            };
        }

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
        public Xml.MethodCall DumpInstanceMethod(uint id, object target, object returnValue, string signature)
        {
            var call = new Xml.MethodCall()
            {
                Id = id,
                Signature = signature,
                Type = Xml.MethodCallType.Instance,
                PostCallInstance = objDumper.DumpObject(target),
            };

            if (returnValue.IsPrimitive())
            {
                call.ReturnValue.Item = objDumper.DumpPrimitive(returnValue);
            }
            else
            {
                call.ReturnValue.Item = objDumper.DumpObject(returnValue);
            }

            return call;
        }

        #endregion
    }
}
