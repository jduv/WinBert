using System.Reflection;
namespace Arktos.WinBert.Environment
{
    /// <summary>
    /// Defines a basic wrapper around an assembly for CCI and reflection based assembly representations.
    /// It requires that an assembly have a location on disk, and because of this dynamic assemblies built in
    /// memory should be written to disk before wrapped in an implementation of this interface.
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
