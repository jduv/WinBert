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
    using System.Collections.Generic;
    using Arktos.WinBert.Exceptions;
    using Arktos.WinBert.Extensions;

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
            // This is where we should catch exceptions if they occur.
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
            private IDictionary<uint, LinkedListNode<IOperation>> operationLookup;
            private LinkedList<IOperation> operationStack;
            private ILocalDefinition target;

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
                // Initialize operation metadata.
                this.InitializeOperationMetadata(methodBody);

                // Should only ever be one as checked in the test rewriter that calls this guy.
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
                if (operation.IsOperationInTryBlock(this.handlerBounds))
                {
                    // Instrument standard Randoop method body patterns.
                    this.InstrumentTryBlockOperation(operation);
                }
                else if (operation.IsRet())
                {
                    // Hook in before ret and call EndTest.
                    this.Generator.Emit(OperationCode.Call, this.EndTestDefinition);
                    base.EmitOperation(operation);
                }
                else
                {
                    // Emit operation, nothing special here.
                    base.EmitOperation(operation);
                }
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Sets up internal data types so that operations and their neighbors can be efficiently inspected
            /// as needed. We should have a hash map for O(1) lookups and a linked list for quick access to
            /// peeks for previous and next IL instructions.
            /// </summary>
            /// <param name="methodBody">
            /// The method body to process.
            /// </param>
            private void InitializeOperationMetadata(IMethodBody methodBody)
            {
                this.operationLookup = new Dictionary<uint, LinkedListNode<IOperation>>();
                this.operationStack = new LinkedList<IOperation>();
                foreach (var operation in methodBody.Operations)
                {
                    var listNode = this.operationStack.AddLast(operation);
                    this.operationLookup[operation.Offset] = listNode;
                }
            }

            /// <summary>
            /// Instruments the try block of a Randoop test.
            /// </summary>
            /// <param name="operation">
            /// The operation to process.
            /// </param>
            private void InstrumentTryBlockOperation(IOperation operation)
            {
                if (operation.IsNewObj())
                {
                    // Set local target so other calls can use it.
                    var nextNode = this.operationLookup[operation.Offset];
                    var nextOp = nextNode != null ? nextNode.Value : null;
                    this.target = nextOp.Value as ILocalDefinition;
                }
                else if (operation.IsStoreLocal())
                {
                    // Handle store local. Couple of cases in here.
                    this.InstrumentCtorOrMethodCallWithReturn(operation);
                }
                else if (operation.IsCallVirt())
                {
                    // Handle call virtual in cases where store local doesn't (i.e. void method calls).
                    this.InstrumentVoidMethodCall(operation);
                }
                else
                {
                    // In all other cases, continue as normal.
                    base.EmitOperation(operation);
                }
            }

            /// <summary>
            /// Handles a store local operation. This method should handle cases for object constructor 
            /// calls and non-void method call patterns.
            /// </summary>
            /// <param name="storeLocal">
            /// The operation to handle.
            /// </param>
            private void InstrumentCtorOrMethodCallWithReturn(IOperation storeLocal)
            {
                // First, emit the store local
                base.EmitOperation(storeLocal);

                var previousNode = this.operationLookup[storeLocal.Offset].Previous;
                var previousOp = previousNode != null ? previousNode.Value : null;
                var methodDef = previousOp != null ? previousOp.Value as IMethodReference : null;
                var returnValue = storeLocal.Value as ILocalDefinition; // Grab what we stored.

                // This hinges on the previous operation having a method definition as it's value. This
                // will occur in two cases: 1. Constructor call, i.e. newobj, and 2. a virtual object call,
                // i.e. callvirt.
                if (previousOp != null && methodDef != null)
                {
                    if (previousOp.IsNewObj())
                    {
                        // Instrument .ctor call
                        this.Generator.Emit(OperationCode.Ldloc, this.target);
                        this.Generator.Emit(OperationCode.Ldstr, methodDef.Name.Value);
                        this.Generator.Emit(OperationCode.Call, this.RecordVoidInstanceMethodDefinition);
                    }
                    else if (previousOp.IsCallVirt() && returnValue != null)
                    {
                        // Previous call was a call virtual and the current operation is a store local
                        // by assumption. That means we're storing a return value onto the stack.
                        this.Generator.Emit(OperationCode.Ldloc, this.target);
                        this.Generator.Emit(OperationCode.Ldloc, returnValue);
                        this.Generator.Emit(OperationCode.Ldstr, methodDef.Name.Value);
                        this.Generator.Emit(OperationCode.Call, this.RecordInstanceMethodDefinition);
                    }
                }
            }

            /// <summary>
            /// Specifically handles void method call patterns.
            /// </summary>
            /// <param name="operation">
            /// The operation to handle.
            /// </param>
            private void InstrumentVoidMethodCall(IOperation operation)
            {
                base.EmitOperation(operation);

                var nextNode = this.operationLookup[operation.Offset].Next;
                var nextOp = nextNode != null ? nextNode.Value : null;
                var methodDef = operation.Value as IMethodReference;

                // If the next operation after this callvirt is not a store local, then we may
                // assume that no return values are being saved to the stack. Hence, 
                // we have a void method call here.
                if (nextOp != null && !nextOp.IsStoreLocal() && methodDef != null)
                {
                    this.Generator.Emit(OperationCode.Ldloc, this.target);
                    this.Generator.Emit(OperationCode.Ldstr, methodDef.Name.Value);
                    this.Generator.Emit(OperationCode.Call, this.RecordVoidInstanceMethodDefinition);
                }
            }

            #endregion
        }

        #endregion
    }
}
