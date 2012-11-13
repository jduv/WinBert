namespace Arktos.WinBert.Differencing.Cci
{
    using System.Collections.Generic;
    using Microsoft.Cci;

    /// <summary>
    /// Implementations of this interface represent a type difference result in the CCI metadata
    /// context.
    /// </summary>
    public interface ICciTypeDifferenceResult : IDifferenceResult<INamedTypeDefinition>
    {
        #region Properties

        /// <summary>
        /// Gets a list of added fields.
        /// </summary>
        IList<IFieldDefinition> AddedFields { get; }

        /// <summary>
        /// Gets a list of all changed methods.
        /// </summary>
        IList<IMethodDefinition> Methods { get; }

        /// <summary>
        /// Gets a list of removed fields.
        /// </summary>
        IList<IFieldDefinition> RemovedFields { get; }

        #endregion
    }
}
