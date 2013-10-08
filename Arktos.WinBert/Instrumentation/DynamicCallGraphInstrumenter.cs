namespace Arktos.WinBert.Instrumentation
{
    using System;
    using System.Diagnostics;
    using AppDomainToolkit;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;

    /// <summary>
    /// Instruments the target assembly with calls to the <see cref="TestUtil.AddMethodToDynamicCallGraph"/> method.
    /// Note that once this class has instrumented the target assembly, it will only work in context of the WinBert test anslysis
    /// framework. The changes made require the state of the <see cref="TestUtil"/> class to be very specific in order for the
    /// assembly to be executable. This state is managed by the set of injections by the <see cref="RandoopTestRewriter"/>
    /// class.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicCallGraphInstrumenter"/> class.
        /// </summary>
        /// <param name="host">
        /// The host to initialize with.
        /// </param>
        /// <returns>
        /// A new instance of the <see cref="DynamicCallGraphInstrumenter"/> class.
        /// </returns>
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
        /// method that writes it's signature using the <see cref="TestUtil"/> class.
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

            foreach (var operation in methodBody.Operations)
            {
                Debug.WriteLine("OpCode: " + operation.OperationCode);
                Debug.WriteLine("Value type: " + operation.Value);
            }

            return methodBody;
        }

        #endregion
    }
}
