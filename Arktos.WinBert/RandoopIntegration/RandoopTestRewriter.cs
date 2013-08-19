namespace Arktos.WinBert.RandoopIntegration
{
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using Arktos.WinBert.Instrumentation;
    using AppDomainToolkit;
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Class that rewrites a generated randoop test.
    /// </summary>
    public class RandoopTestRewriter : MetadataRewriter
    {
        #region Fields & Constants

        private readonly string testMethodName;

        #endregion

        #region Constructors & Destructors

        private RandoopTestRewriter(string testMethodName, IMetadataHost host)
            : base(host)
        {
            this.testMethodName = testMethodName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates an instance of the RandoopTestRewriter class.
        /// </summary>
        /// <param name="testMethodName">
        /// The name of the test method to look for.
        /// </param>
        /// <returns>An instance of the RandoopTestRewriter class.</returns>
        public static RandoopTestRewriter Create(string testMethodName)
        {
            return Create(testMethodName, new PeReader.DefaultHost());
        }

        /// <summary>
        /// Creates an instance of the RandoopTestRewriter class.
        /// </summary>
        /// <param name="testMethodName">
        /// The name of the test method to look for.
        /// </param>
        /// <param name="host">
        /// The metadata host.
        /// </param>
        /// <returns>An instance of the RandoopTestRewriter class.</returns>
        public static RandoopTestRewriter Create(string testMethodName, IMetadataHost host)
        {
            if (string.IsNullOrWhiteSpace(testMethodName))
            {
                throw new ArgumentException("Test method name cannot be null or white space!");
            }

            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            return new RandoopTestRewriter(testMethodName, host);
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
        /// Rewrites the target method body. Method bodies will be assumed to follow the Randoop
        /// pattern of a target object instantiation followed by some number of method calls on 
        /// that object. All of this will usually be wrapped in a try/catch block, but the rewriter
        /// will try to ignore those as best it can.
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
