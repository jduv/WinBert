namespace Arktos.WinBert.Instrumentation
{
    using System;
    using System.Diagnostics;
    using AppDomainToolkit;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;

    public class DynamicCallGraphInstrumenter : MetadataRewriter
    {
        #region Fields & Constants

        private readonly string testMethodName;

        #endregion

        #region Constructors & Destructors

        private DynamicCallGraphInstrumenter(IMetadataHost host)
            : base(host)
        {
        }

        #endregion

        #region Public Methods

        public static DynamicCallGraphInstrumenter Create(IMetadataHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            return new DynamicCallGraphInstrumenter(host);
        }

        /// <summary>
        /// Rewrites an instrumentation target.
        /// </summary>
        /// <param name="target">
        /// The target to rewrite.
        /// </param>
        /// <returns>
        /// An assembly target pointing to the assembly inside the rewritten instrumentation
        /// target.
        /// </returns>
        public IAssemblyTarget Rewrite(IInstrumentationTarget target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            this.RewriteChildren(target.MutableAssembly);
            return target.Save();
        }

        /// <summary>
        /// Rewrites the target method body. This class will only insert a single call into the beginning of the
        /// method that writes it's signature using the TestUtil class.
        /// </summary>
        /// <param name="methodBody">
        /// The method body to rewrite.
        /// </param>
        /// <returns>
        /// The rewritten method body.
        /// </returns>
        public override IMethodBody Rewrite(IMethodBody methodBody)
        {
            if (methodBody == null)
            {
                throw new ArgumentNullException("methodBody");
            }

            if (methodBody.MethodDefinition.Name.Value.Equals(this.testMethodName))
            {
                foreach (var operation in methodBody.Operations)
                {
                    Debug.WriteLine("OpCode: " + operation.OperationCode);
                    Debug.WriteLine("Value type: " + operation.Value);
                }
            }

            return methodBody;
        }

        #endregion
    }
}
