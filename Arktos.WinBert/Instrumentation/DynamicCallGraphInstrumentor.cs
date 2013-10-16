namespace Arktos.WinBert.Instrumentation
{
    using System;
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
    public class DynamicCallGraphInstrumentor : MetadataRewriter
    {
        #region Fields & Constants

        private readonly IAssembly winbertCore;
        private ILRewriter rewriter;

        #endregion

        #region Constructors & Destructors

        public DynamicCallGraphInstrumentor(IMetadataHost host)
            : base(host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            // Load winbert core
            this.winbertCore = (IAssembly)host.LoadUnitFrom(this.GetType().Assembly.Location);
        }

        #endregion

        #region Public Methods

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

            // New up the rewriter
            this.rewriter = new DCGMethodInjector(target.Host, target.LocalScopeProvider, target.SourceLocationProvider, this.winbertCore);

            // Inject calls
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
            if (this.rewriter == null)
            {
                throw new InvalidOperationException("Unable to rewrite method body. Call Rewrite(IInstrumentationTarget target) instead of calling this directly.");
            }

            return this.rewriter.Rewrite(methodBody);
        }

        #endregion

        #region Private Classes

        /// <summary>
        /// This class encapuslates the call graph method injection logic.
        /// </summary>
        private class DCGMethodInjector : TestUtilMethodInjector
        {
            #region Constructors & Destructors

            public DCGMethodInjector(
                IMetadataHost host,
                ILocalScopeProvider localScopeProvider,
                ISourceLocationProvider sourceLocationProvider,
                IAssembly winbertCore)
                : base(host, localScopeProvider, sourceLocationProvider, winbertCore)
            {
            }

            #endregion

            #region Protected Methods

            /// <inheritdoc />
            /// <remarks>
            /// Simply grabs the method signature and emits some operations to load it and execute
            /// a method call to the <see cref="TestUtil"/> call graph method definition.
            /// </remarks>
            protected override void EmitMethodBody(IMethodBody methodBody)
            {
                var signature = MemberHelper.GetMethodSignature(methodBody.MethodDefinition);
                this.Generator.Emit(OperationCode.Ldstr, signature);
                this.Generator.Emit(OperationCode.Call, this.AddMethodToDynamicCallGraphDefinition);
                base.EmitMethodBody(methodBody);
            }

            #endregion
        }

        #endregion
    }
}
