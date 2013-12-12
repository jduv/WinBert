namespace Arktos.WinBert.Differencing
{
    using AppDomainToolkit;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the contract for a difference result for an assembly.
    /// </summary>
    public interface IAssemblyDifference : IDifferenceResult
    {
        #region Properties

        /// <summary>
        ///   Gets a list of type differences for the assembly.
        /// </summary>
        IEnumerable<ITypeDifference> TypeDifferences { get; }

        /// <summary>
        /// Gets the new assembly target.
        /// </summary>
        IAssemblyTarget NewAssemblyTarget { get; }

        /// <summary>
        /// Gets the old assembly target.
        /// </summary>
        IAssemblyTarget OldAssemblyTarget { get; }

        /// <summary>
        /// Gets a type difference for the target type given a fully qualified name.
        /// </summary>
        /// <param name="fullName">
        /// The fully qualified name of the type to grab a difference for.
        /// </param>
        /// <returns>
        /// A type difference implementation, or null if not found.
        /// </returns>
        ITypeDifference this[string fullName] { get; }

        #endregion
    }
}