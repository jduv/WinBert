namespace Arktos.WinBert.RandoopIntegration
{
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using Arktos.WinBert.Instrumentation;
    using AppDomainToolkit;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Class that rewrites a generated randoop test.
    /// </summary>
    public class RandoopTestRewriter : MetadataRewriter
    {
        #region Fields & Constants

        private readonly string testMethodName;
        private readonly IAssembly winbertCore;
        private ILRewriter rewriter;

        #endregion

        #region Constructors & Destructors

        public RandoopTestRewriter(string testMethodName)
            : this(testMethodName, new PeReader.DefaultHost())
        {
        }

        public RandoopTestRewriter(string testMethodName, IMetadataHost host)
            : base(host)
        {
            if (string.IsNullOrWhiteSpace(testMethodName))
            {
                throw new ArgumentException("Test method name cannot be null or white space!");
            }

            this.testMethodName = testMethodName;

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
            var uniqueId = Guid.NewGuid().ToString();
            this.rewriter = new RandoopTestILRewriter(
                target.Host,
                target.LocalScopeProvider,
                target.SourceLocationProvider,
                this.winbertCore);

            this.RewriteChildren(target.MutableAssembly);
            return target.Save();
        }

        /// <summary>
        /// Rewrites the target method body. Method bodies will be assumed to follow the Randoop
        /// pattern of a target object instantiation followed by some number of method calls on 
        /// that object. All of this will usually be wrapped in a single try/catch block. If we detect that
        /// there are multiple exception flows in the test we won't instrument it because that configuration
        /// is unsupported.
        /// </summary>
        /// <param name="methodBody">
        /// The method body to rewrite.
        /// </param>
        /// <returns>
        /// The rewritten method body.
        /// </returns>
        public override IMethodBody Rewrite(IMethodBody methodBody)
        {
            return methodBody.OperationExceptionInformation.Count() == 1 ? this.rewriter.Rewrite(methodBody) : base.Rewrite(methodBody);
        }

        #endregion

        #region Private classes

        /// <summary>
        /// This class encapsulates test util method injector logic.
        /// </summary>
        private class RandoopTestILRewriter : TestUtilMethodInjector
        {
            #region Fields  & Constants 

            private IOperationExceptionInformation handlerBounds;

            #endregion

            #region Constructors & Destructors

            public RandoopTestILRewriter(
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
            /// method calls to the <see cref="TestUtil"/> class.
            /// </remarks>
            protected override void EmitMethodBody(IMethodBody methodBody)
            {
                // Should only ever be one as checked in the test rewriter.
                this.handlerBounds = methodBody.OperationExceptionInformation.First();
                
                // Emit start test
                this.Generator.Emit(OperationCode.Call, this.StartTestDefinition);
                base.EmitMethodBody(methodBody);
            }

            /// <inheritdoc />
            /// <remarks>
            /// Emits operations to inject <see cref="TestUtil"/> method calls into the target method body
            /// that adhere to a Randoop test injection pattern.
            /// </remarks>
            protected override void EmitOperation(IOperation operation)
            {
                if (this.IsOperationInTryBlock(operation))
                {
                    this.InstrumentTryBlockOperation(operation);
                }
                else
                {
                    base.EmitOperation(operation);
                }
            }

            #endregion

            #region Private Methods

            private void InstrumentTryBlockOperation(IOperation operation)
            {
                switch (operation.OperationCode)
                {
                    case OperationCode.Ret:
                        // Hook in before ret and call EndTest.
                        this.Generator.Emit(OperationCode.Call, this.EndTestDefinition);
                        base.EmitOperation(operation);
                        break;
                    case OperationCode.Newobj:
                        Debug.WriteLine("Found newObj");
                        Debug.WriteLine(operation.Value);
                        break;
                }

                base.EmitOperation(operation);
            }

            /// <summary>
            /// Is the target operation in the try block of the test?
            /// </summary>
            /// <param name="operation">
            /// The operation to test.
            /// </param>
            /// <returns>
            /// True if the operation is in the test try block, false otherwise.
            /// </returns>
            private bool IsOperationInTryBlock(IOperation operation)
            {
                return operation.Offset >= this.handlerBounds.TryStartOffset && operation.Offset <= this.handlerBounds.TryEndOffset;
            }

            /// <summary>
            /// Is the target operation in the catch block of the test?
            /// </summary>
            /// <param name="operation">
            /// The operation to test.
            /// </param>
            /// <returns>
            /// True if the operation is in the test catch block, false otherwise.
            /// </returns>
            private bool IsOperationInCatchBlock(IOperation operation)
            {
                return operation.Offset >= this.handlerBounds.HandlerStartOffset && operation.Offset <= this.handlerBounds.HandlerEndOffset;
            }

            #endregion
        }

        #endregion
    }
}
