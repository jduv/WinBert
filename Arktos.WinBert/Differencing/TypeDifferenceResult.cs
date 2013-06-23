namespace Arktos.WinBert.Differencing
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Cci;

    /// <summary>
    /// This class represents a difference result between two types. The differences between the
    /// two types is represented by a list of method names that have changed. Note that this does not
    /// truly represent a 'diff' between the two types, as we are only interested in the methods inside
    /// the two types that 1) have the same signature and 2) have a different body. This class should always 
    /// be marked serializable to avoid issues with app domain lifecycles. Never pass the raw types, 
    /// however back and forth across application domains--you'll pollute the current app domain.
    /// </summary>
    [Serializable]
    public sealed class TypeDifferenceResult : ITypeDifferenceResult
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the TypeDifferenceResult class.
        /// </summary>
        /// <param name="oldType">
        /// The old type.
        /// </param>
        /// <param name="newType">
        /// The new type.
        /// </param>
        public TypeDifferenceResult(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty!");
            }

            this.Name = name;
            this.Methods = new List<string>();
            this.RemovedFields = new List<string>();
            this.AddedFields = new List<string>();
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public bool IsDifferent
        {
            get
            {
                return this.Methods.Count > 0 || this.AddedFields.Count > 0 || this.RemovedFields.Count > 0;
            }
        }

        /// <inheritdoc />
        public int ItemsCompared { get; private set; }

        /// <inheritdoc />
        public string Name { get; private set; }

        /// <inheritdoc />
        public IList<string> AddedFields { get; private set; }

        /// <inheritdoc />
        public IList<string> Methods { get; private set; }

        /// <inheritdoc />
        public IList<string> RemovedFields { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new TypeDifferenceResult instance from the target type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// A new TypeDifferenceResult from the target type.
        /// </returns>
        public static TypeDifferenceResult FromType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return new TypeDifferenceResult(type.Name);
        }

        #endregion
    }
}