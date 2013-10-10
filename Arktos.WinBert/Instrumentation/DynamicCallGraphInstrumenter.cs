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

        private static readonly string InjectedMethodName = "AddMethodToDynamicCallGraph";
        private readonly IMethodDefinition cgMethod;
        private ILRewriter rewriter;

        #endregion

        #region Constructors & Destructors

        private DynamicCallGraphInstrumenter(IMetadataHost host)
            : base(host)
        {
            // Load winbert core
            var winBertCore = (IAssembly)host.LoadUnitFrom(this.GetType().Assembly.Location);

            // Grab the test utility type definition.
            var testUtilDefinition = (INamespaceTypeDefinition)UnitHelper.FindType(host.NameTable, winBertCore, typeof(TestUtil).FullName);

            // Grab the call graph method to be injected
            this.cgMethod = TypeHelper.GetMethod(testUtilDefinition, host.NameTable.GetNameFor(InjectedMethodName), host.PlatformType.SystemString);
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

            // New up the rewriter
            this.rewriter = new CallGraphMethodInjector(target.Host, target.LocalScopeProvider, target.SourceLocationProvider, this.cgMethod);

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
            return rewriter.Rewrite(methodBody);
        }

        #endregion

        #region Private Inner Classes

        /// <summary>
        /// This class encapuslates the call graph method injection logic.
        /// </summary>
        private class CallGraphMethodInjector : ILRewriter
        {

            #region Constructors & Destructors

            /// <summary>
            /// Creates a new instance of the CallGraphMethodInjector class.
            /// </summary>
            /// <param name="host">
            /// The metadata host.
            /// </param>
            /// <param name="localScopeProvider">
            /// The local scope provider.
            /// </param>
            /// <param name="sourceLocationProvider">
            /// The source location provider.
            /// </param>
            /// <param name="cgMethodDefinition">
            /// The call graph method definition.
            /// </param>
            public CallGraphMethodInjector(
                IMetadataHost host,
                ILocalScopeProvider localScopeProvider,
                ISourceLocationProvider sourceLocationProvider,
                IMethodDefinition cgMethodDefinition)
                : base(host, localScopeProvider, sourceLocationProvider)
            {
                if (cgMethodDefinition == null)
                {
                    throw new ArgumentNullException("cgMethodDefinition");
                }

                this.CallGraphMethodDefinition = cgMethodDefinition;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the call graph method definition.
            /// </summary>
            protected IMethodDefinition CallGraphMethodDefinition { get; private set; }

            #endregion

            #region Protected Methods                                                                                                                               

            /// <inheritdoc />
            /// <remarks>
            /// Simply grabs the method signature and emits some operations to load it and execute
            /// a method call to the <see cref="TestUtil"/> call graph method definition passed in on 
            /// the constructor.    
            /// </remarks>
            protected override void EmitMethodBody(IMethodBody methodBody)
            {
                var signature = MemberHelper.GetMethodSignature(methodBody.MethodDefinition);
                this.Generator.Emit(OperationCode.Ldstr, signature);
                this.Generator.Emit(OperationCode.Call, this.CallGraphMethodDefinition);
                base.EmitMethodBody(methodBody);
            }

            #endregion
        }

        #endregion
    }
}
