namespace Arktos.WinBert.RandoopIntegration
{
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using Arktos.WinBert.Instrumentation;
    using AppDomainToolkit;

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

        public static RandoopTestRewriter Create(string testMethodName)
        {
            return Create(testMethodName, new PeReader.DefaultHost());
        }

        public static RandoopTestRewriter Create(string testMethodName, IMetadataHost host)
        {
            return new RandoopTestRewriter(testMethodName, host);
        }

        public IAssemblyTarget Rewrite(IInstrumentationTarget target)
        {
            return target.Save();
        }

        public override IMethodBody Rewrite(IMethodBody methodBody)
        {
            if (methodBody.MethodDefinition.Name.Value.Equals(this.testMethodName))
            {
                // Rewrite
            }

            return methodBody;
        }

        #endregion
    }
}
