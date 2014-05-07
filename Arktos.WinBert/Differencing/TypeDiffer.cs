namespace Arktos.WinBert.Differencing
{
    using Arktos.WinBert.Xml;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// This simple type differencing engine will take two types and figure out the difference between them.
    /// </summary>
    public sealed class TypeDiffer : MarshalByRefObject, IDifferenceEngine<Type, ITypeDifference>
    {
        #region Fields & Constants

        private readonly IEnumerable<DiffIgnoreTarget> ignoreTargets;

        #endregion

        #region Constructors and Destructors

        public TypeDiffer(IEnumerable<DiffIgnoreTarget> ignoreTargets)
        {
            if (ignoreTargets == null)
            {
                throw new ArgumentNullException("ignoreTargets");
            }

            this.ignoreTargets = ignoreTargets.Where(x => x.Type == DiffIgnoreType.Method).ToList();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Implementation of the Diff method required by the IDifferenceEngine interface. This method will perform
        ///   a very rudimentary diff on the two passed in types
        /// </summary>
        /// <param name="oldObject">
        /// The first object to compare.
        /// </param>
        /// <param name="newObject">
        /// The second object to compare.
        /// </param>
        /// <returns>
        /// An IDifferenceResult implementation that contains all the differences between the target types in
        /// a hierarchical manner. <see cref="TypeDifference"/>
        /// </returns>
        public ITypeDifference Diff(Type oldObject, Type newObject)
        {
            if (oldObject == null)
            {
                throw new ArgumentNullException("oldObject");
            }
            if (newObject == null)
            {
                throw new ArgumentNullException("newObject");
            }

            var oldTypeDict = oldObject.GetMethods().Where(x => x.DeclaringType.Name == oldObject.Name).ToDictionary(x => x.DeclaringType.FullName + "." + x.Name);
            var newTypes = newObject.GetMethods().Where(x => x.DeclaringType.Name == newObject.Name &&
                !this.ignoreTargets.Any(y => y.Name.Equals(x.Name)));

            var methods = new List<string>();
            foreach (var method in newTypes.Select(x => new { FullSig = x.DeclaringType + "." + x.Name, MethodInfo = x }))
            {
                if (oldTypeDict.ContainsKey(method.FullSig))
                {
                    var toCompare = oldTypeDict[method.FullSig];
                    if (AreDifferent(method.MethodInfo.GetMethodBody(), toCompare.GetMethodBody()))
                    {
                        methods.Add(method.FullSig);
                    }
                }
            }

            return new TypeDifference(newObject, methods);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Compares the two target method bodies and determines if they are different. This method performs a
        /// binary difference between them by converting both method bodies into byte arrays and then performing a
        /// bit by bit comparison between the resulting sets.
        /// </summary>
        /// <param name="first">
        /// The first method body to compare.
        /// </param>
        /// <param name="second">
        /// The second method body to compare.
        /// </param>
        /// <returns>
        /// True if the method bodies are different, false otherwise.
        /// </returns>
        private bool AreDifferent(MethodBody first, MethodBody second)
        {
            bool areDifferent = false;
            if (first != null && second != null)
            {
                var firstBodyBits = first.GetILAsByteArray();
                var secondBodyBits = second.GetILAsByteArray();

                // Stack size, body bits length, and local signature metadata might be different.
                if (firstBodyBits.Length != secondBodyBits.Length ||
                    first.MaxStackSize != second.MaxStackSize ||
                    first.LocalSignatureMetadataToken != second.LocalSignatureMetadataToken)
                {
                    areDifferent = true;
                }
                else
                {
                    // While loop is more readable and avoids break/return mid loop.
                    int i = 0;
                    while (i < firstBodyBits.Length && !areDifferent)
                    {
                        if (firstBodyBits[i] != secondBodyBits[i])
                        {
                            areDifferent = true;
                        }

                        i++;
                    }
                }
            }

            return areDifferent;
        }

        #endregion
    }
}