namespace Arktos.WinBert.Instrumentation
{
    using System;
    using System.Collections.Generic;
    using Arktos.WinBert.Extensions;

    /// <summary>
    /// Logs method calls of various types. In essence here we have factory methods for Xml.MethodCall
    /// objects.
    /// </summary>
    public sealed class MethodCallDumper : IMethodCallDumper
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

        /// <inheritdoc/>
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
                PostCallInstance = ObjDumper.DumpObject(target),
                DynamicCallGraph = new List<Xml.CallGraphNode>()
            };
        }

        /// <inheritdoc/>
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
                ReturnValue = new Xml.Value(),
                DynamicCallGraph = new List<Xml.CallGraphNode>()
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
