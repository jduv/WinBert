namespace Arktos.WinBert.Differencing
{
    using System.Collections.Generic;
    using Arktos.WinBert.Environment;

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
        IAssemblyTarget NewAssembly { get; }

        /// <summary>
        /// Gets the old assembly target.
        /// </summary>
        IAssemblyTarget OldAssembly { get; }

        #endregion
    }
}