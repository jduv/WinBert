namespace Arktos.WinBert.Instrumentation
{
    using Arktos.WinBert.Environment;

    public interface ITestInstrumenter
    {
        #region Methods

        public IAssemblyTarget InstrumentTests(IAssemblyTarget toInstrument);

        #endregion
    }
}
