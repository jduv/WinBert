namespace Arktos.WinBert.Differencing
{
    using System.Collections.Generic;
    using Arktos.WinBert.Environment;
    using System.Reflection;

    /// <summary>
    /// Defines the contract for a difference result for an assembly.
    /// </summary>
    public interface IAssemblyDifferenceResult : IDifferenceResult
    {
        #region Properties

        /// <summary>
        ///   Gets a list of type differences for the assembly.
        /// </summary>
        IList<ITypeDifferenceResult> TypeDifferences { get; }

        /// <summary>
        /// Gets the new assembly target.
        /// </summary>
        AssemblyTarget NewAssemblyTarget { get; }

        /// <summary>
        /// Gets the old assembly target.
        /// </summary>
        AssemblyTarget OldAssemblyTarget { get; }

        #endregion
    }
}