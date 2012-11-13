namespace Arktos.WinBert.Differencing.Cci
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Cci;

    /// <summary>
    /// Represents an assembly difference result in the CCI metadata context.
    /// </summary>
    public class CciAssemblyDifferenceResult : ICciAssemblyDifferenceResult
    {
        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the CciAssemblyDifferenceResult class.
        /// </summary>
        /// <param name="newObject">
        /// The new assembly object.
        /// </param>
        /// <param name="oldObject">
        /// The old assembly object.
        /// </param>
        public CciAssemblyDifferenceResult(IAssembly oldObject, IAssembly newObject)
        {
            if (newObject == null)
            {
                throw new ArgumentNullException("newObject");
            }

            if (oldObject == null)
            {
                throw new ArgumentNullException("oldObject");
            }

            this.NewObject = newObject;
            this.OldObject = oldObject;
            this.TypeDifferences = new List<ICciTypeDifferenceResult>();
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public bool IsDifferent 
        {
            get
            {
                return this.TypeDifferences.Count > 0;
            }
        }

        /// <inheritdoc />
        public IList<ICciTypeDifferenceResult> TypeDifferences { get; private set; }

        /// <inheritdoc />
        public IAssembly NewObject { get; private set; }

        /// <inheritdoc />
        public IAssembly OldObject { get; private set; }

        #endregion
    }
}
