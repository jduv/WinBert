namespace Arktos.WinBert.Differencing
{
    /// <summary>
    /// Defines a basic wrapper around an assembly for CCI and reflection based assembly representations.
    /// </summary>
    public interface IAssemblyTarget
    {
        #region Properties

        /// <summary>
        /// The location on disk of the assembly.
        /// </summary>
        string Location { get; }

        #endregion
    }
}
