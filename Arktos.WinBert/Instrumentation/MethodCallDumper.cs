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

        private readonly IObjectDumper objectDumper;

        #endregion

        #region Constructors & Destructors

        public MethodCallDumper()
            : this(new ObjectDumper())
        {
        }

        public MethodCallDumper(IObjectDumper objectDumper)
        {
            this.objectDumper = objectDumper;
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
                // Instance method target shouldn't be null
                throw new ArgumentNullException("target");
            }

            return new Xml.MethodCall()
            {
                Id = id,
                Signature = signature,
                Type = Xml.MethodCallType.Instance,
                PostCallInstance = this.objectDumper.DumpObject(target),
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
                // Instance method target shouldn't be null
                throw new ArgumentNullException("target");
            }

            var call = new Xml.MethodCall()
            {
                Id = id,
                Signature = signature,
                Type = Xml.MethodCallType.Instance,
                PostCallInstance = this.objectDumper.DumpObject(target),
                ReturnValue = new Xml.Value(),
                DynamicCallGraph = new List<Xml.CallGraphNode>()
            };

            if (returnValue.IsPrimitive())
            {
                call.ReturnValue.Item = this.objectDumper.DumpPrimitive(returnValue);
            }
            else
            {
                call.ReturnValue.Item = this.objectDumper.DumpObject(returnValue);
            }

            return call;
        }

        #endregion
    }
}
