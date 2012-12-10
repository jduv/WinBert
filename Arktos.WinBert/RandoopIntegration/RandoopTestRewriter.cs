namespace Arktos.WinBert.RandoopIntegration
{
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;

    public class RandoopTestRewriter : MetadataRewriter
    {
        #region Fields & Constants

        private readonly string testMethodName;

        #endregion

        #region Constructors & Destructors

        public RandoopTestRewriter(string testMethodName)
        {
            this.testMethodName = testMethodName;
        }

        #endregion

        public override IMethodBody Rewrite(IMethodBody methodBody)
        {
            if (methodBody.MethodDefinition.Name.Value.Equals(this.testMethodName))
            {
                // Rewrite
            }
        }
    }
}
