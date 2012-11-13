namespace Arktos.WinBert.Differencing.Cci
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Cci;

    /// <summary>
    /// Represents a type difference in a CCI metadata context.
    /// </summary>
    public class CciTypeDifferenceResult : ICciTypeDifferenceResult
    {
        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the CciTypeDifferenceResult class.
        /// </summary>
        /// <param name="newType">
        /// The new type.
        /// </param>
        /// <param name="oldType">
        /// The old type.
        /// </param>
        public CciTypeDifferenceResult(INamedTypeDefinition oldType, INamedTypeDefinition newType)
        {
            if (oldType == null)
            {
                throw new ArgumentNullException("oldType");
            }

            if (newType == null)
            {
                throw new ArgumentNullException("newType");
            }

            this.NewObject = newType;
            this.OldObject = oldType;
            this.AddedFields = new List<IFieldDefinition>();
            this.Methods = new List<IMethodDefinition>();
            this.RemovedFields = new List<IFieldDefinition>();
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public bool IsDifferent
        {
            get
            {
                return this.Methods.Count > 0;
            }
        }

        /// <inheritdoc />
        public IList<IFieldDefinition> AddedFields { get; private set; }

        /// <inheritdoc />
        public IList<IMethodDefinition> Methods { get; private set; }

        /// <inheritdoc />
        public IList<IFieldDefinition> RemovedFields { get; private set; }

        /// <inheritdoc />
        public INamedTypeDefinition NewObject { get; private set; }

        /// <inheritdoc />
        public INamedTypeDefinition OldObject { get; private set; }

        #endregion
    }
}
