namespace Arktos.WinBert.Instrumentation
{
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;

    public sealed class DynamicCallGraphInstrumenter : MetadataRewriter
    {
        #region Constructors & Destructors

        public DynamicCallGraphInstrumenter(IMetadataHost host)
            : base(host)
        {
        }

        #endregion

        #region Public Methods

        public static DynamicCallGraphInstrumenter Create()
        {
            return new DynamicCallGraphInstrumenter(new PeReader.DefaultHost());
        }

        public static DynamicCallGraphInstrumenter Create(IMetadataHost host)
        {
            return new DynamicCallGraphInstrumenter(host);
        }

        public override IMethodBody Rewrite(IMethodBody methodBody)
        {
            return methodBody;
        }

        #endregion
    }
}
