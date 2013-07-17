namespace Arktos.WinBert.Instrumentation
{
    using System;
    using Arktos.WinBert.Extensions;

    /// <summary>
    /// Logs method calls of various types. In essence here we have factory methods for Xml.MethodCall
    /// objects.
    /// </summary>
    public sealed class MethodCallDumper
    {
        #region Constants & Fields

        private static readonly ObjectDumper ObjDumper;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static MethodCallDumper()
        {
            ObjDumper = new ObjectDumper();
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
            if (string.IsNullOrEmpty(signature))
            {
                throw new ArgumentException("Signature cannot be null or empty!");
            }
            if (target == null)
            {
                // Instance method target shouldn't be null.....
                throw new ArgumentNullException("target");
            }

            return new Xml.MethodCall()
            {
                Id = id,
                Signature = signature,
                Type = Xml.MethodCallType.Instance,
                PostCallInstance = ObjDumper.DumpObject(target)
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
            if (string.IsNullOrEmpty(signature))
            {
                throw new ArgumentException("Signature cannot be null or empty!");
            }

            if (target == null)
            {
                // Instance method target shouldn't be null.....
                throw new ArgumentNullException("target");
            }

            var call = new Xml.MethodCall()
            {
                Id = id,
                Signature = signature,
                Type = Xml.MethodCallType.Instance,
                PostCallInstance = ObjDumper.DumpObject(target),
                ReturnValue = new Xml.Value()
            };

            if (returnValue.IsPrimitive())
            {
                call.ReturnValue.Item = ObjDumper.DumpPrimitive(returnValue);
            }
            else
            {
                call.ReturnValue.Item = ObjDumper.DumpObject(returnValue);
            }

            return call;
        }

        #endregion
    }
}
