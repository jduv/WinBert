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
                var testName = TypeHelper.GetTypeName(methodBody.MethodDefinition.ContainingType);

                // modify max stack if applicable, should be at least three.
                ushort three = 3;
                this.maxStack = this.maxStack < three ? three : this.maxStack;

                this.Generator.Emit(OperationCode.Ldstr, testName);
                this.Generator.Emit(OperationCode.Call, this.StartTestWithNameDefinition);
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
                    // If we see a store local, look at the previous op for a newobj and process.
                    if (operation.IsStoreLocal())
                    {
                        this.FindAndProcessNewObj(operation);
                    }
                    else
                    {
                        // For all other instructions, look at the previous for a callvirt and process.
                        this.FindAndProcessCallVirt(operation);
                    }
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
            /// The method will look at the previous operation and if it is a newobj will insert an instrumentation call and set
            /// the object target local definition on the rewriter.
            /// </summary>
            /// <param name="operation">
            /// The operation to handle.
            /// </param>
            private void FindAndProcessNewObj(IOperation operation)
            {
                // First, emit the store local
                base.EmitOperation(operation);

                var previousNode = this.operationLookup[operation.Offset].Previous;
                var previousOp = previousNode != null ? previousNode.Value : null;
                var methodDef = previousOp != null ? previousOp.Value as IMethodReference : null;

                // Look for newobj instructions.
                if (previousOp != null && previousOp.IsNewObj() && methodDef != null)
                {
                    // Instrument .ctor call
                    this.target = operation.Value as ILocalDefinition;
                    this.Generator.Emit(OperationCode.Ldloc, this.target);
                    this.Generator.Emit(OperationCode.Ldstr, methodDef.Name.Value);
                    this.Generator.Emit(OperationCode.Call, this.RecordVoidInstanceMethodDefinition);
                }
            }

            /// <summary>
            /// This method processes any instruction but looks for a callvirt in the previous spot. If a callvirt is 
            /// discovered, then some logic will kick in to generate the correct instrumentation call depending on 
            /// if the method has a return value or not. Methods with return values will have a pop call immediately 
            /// preceding the call virt, and that instruction will be replaced to a store local before emitting the dumping
            /// code.
            /// </summary>
            /// <param name="operation">
            /// The operation to handle.
            /// </param>
            private void FindAndProcessCallVirt(IOperation operation)
            {
                var previousNode = this.operationLookup[operation.Offset].Previous;
                var previousOp = previousNode != null ? previousNode.Value : null;
                var methodDef = previousOp != null ? previousOp.Value as IMethodReference : null;

                if (previousOp != null && previousOp.IsCallVirt() && methodDef != null)
                {
                    if (operation.IsPop())
                    {
                        // Store the return value into a local definition.
                        var localdef = GenerateReturnValueLocalDefinition(methodDef);

                        // Add to current scope and store.
                        this.Generator.Emit(OperationCode.Stloc, localdef);

                        var signature = MemberHelper.GetMethodSignature(methodDef);
                        this.Generator.Emit(OperationCode.Ldloc, this.target);
                        this.Generator.Emit(OperationCode.Ldloc, localdef);

                        // Box value types.
                        if (localdef.Type.IsValueType)
                        {
                            this.Generator.Emit(OperationCode.Box, localdef.Type);
                        }

                        this.Generator.Emit(OperationCode.Ldstr, signature);
                        this.Generator.Emit(OperationCode.Call, this.RecordInstanceMethodDefinition);
                    }
                    else
                    {
                        // Called a void instance method. Emit dumping method, then current operation.
                        var signature = MemberHelper.GetMethodSignature(methodDef);
                        this.Generator.Emit(OperationCode.Ldloc, this.target);
                        this.Generator.Emit(OperationCode.Ldstr, signature);
                        this.Generator.Emit(OperationCode.Call, this.RecordVoidInstanceMethodDefinition);
                        base.EmitOperation(operation);
                    }
                }
                else
                {
                    base.EmitOperation(operation);
                }
            }

            /// <summary>
            /// Generates a local definition for saving return values to.
            /// </summary>
            /// <param name="methodCalled">
            /// The method called.
            /// </param>
            /// <returns>
            /// The local definition.
            /// </returns>
            private ILocalDefinition GenerateReturnValueLocalDefinition(IMethodReference methodCalled)
            {
                var uniqueId = Guid.NewGuid().ToString().Substring(0, 6);
                var localdef = new LocalDefinition()
                {
                    Name = this.host.NameTable.GetNameFor("ret_" + uniqueId),
                    Type = methodCalled.Type,
                    MethodDefinition = this.Generator.Method
                };

                this.Generator.AddVariableToCurrentScope(localdef);
                this.TrackLocal(localdef);

                return localdef;
            }

            #endregion
        }

        #endregion
    }
}
