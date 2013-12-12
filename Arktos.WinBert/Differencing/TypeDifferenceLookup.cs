namespace Arktos.WinBert.Differencing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Small utiltity type representing a Type along with it's set of method names that
    /// changed. Use this for fast lookup of type to method name relationships when building
    /// out distance numbers for behavioral differences.
    /// </summary>
    public sealed class TypeDifferenceLookup
    {
        #region Fields & Constants

        private readonly HashSet<string> methodNames;

        #endregion

        #region Constructors & Destructors

        public TypeDifferenceLookup(ITypeDifference typeDiff)
        {
            if (typeDiff == null)
            {
                throw new ArgumentNullException("typeDiff");
            }

            this.methodNames = new HashSet<string>();
            foreach (var method in typeDiff.Methods)
            {
                this.methodNames.Add(method);
            }

            this.TypeName = typeDiff.FullName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the parent type's name.
        /// </summary>
        public string TypeName { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines if this type difference contains the target method signature.
        /// </summary>
        /// <param name="methodSignature">The method signature to check.</param>
        /// <returns>
        /// True if this lookup contains the target method signature, false otherwise.
        /// </returns>
        public bool Contains(string methodSignature)
        {
            return this.methodNames.Contains(methodSignature);
        }

        #endregion
    }
}
