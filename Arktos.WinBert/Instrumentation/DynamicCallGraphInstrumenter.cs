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
        private readonly NamespaceTypeDefinition testUtilDef;
        private readonly IMethodDefinition cgMethod;

        #endregion

        #region Constructors & Destructors

        private DynamicCallGraphInstrumenter(IMetadataHost host)
            : base(host)
        {
            // Load winbert core
            var copier = new MetadataDeepCopier(host);
            var winBertCore = (IAssembly)host.LoadUnitFrom(this.GetType().Assembly.Location);

            // Copy test util type definition so it can be "pasted" into the new assembly.
            var originalTestUtilDef = (INamespaceTypeDefinition)UnitHelper.FindType(host.NameTable, winBertCore, typeof(TestUtil).FullName);
            this.testUtilDef = copier.Copy(originalTestUtilDef);

            // Create call graph method to be injected
            this.cgMethod = TypeHelper.GetMethod(this.testUtilDef, host.NameTable.GetNameFor(InjectedMethodName), host.PlatformType.SystemString);
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

            // Copy over the test util namespace and type definition
            this.testUtilDef.ContainingUnitNamespace = target.MutableAssembly.UnitNamespaceRoot;
            target.MutableAssembly.AllTypes.Add(this.testUtilDef);

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
            if (methodBody == null)
            {
                throw new ArgumentNullException("methodBody");
            }

            var signature = MemberHelper.GetMethodSignature(methodBody.MethodDefinition);
            var ilGenerator = new ILGenerator(this.host, methodBody.MethodDefinition);
            ilGenerator.Emit(OperationCode.Ldsfld, signature);
            ilGenerator.Emit(OperationCode.Call, this.cgMethod);
            return methodBody;
        }

        #endregion
    }
}
