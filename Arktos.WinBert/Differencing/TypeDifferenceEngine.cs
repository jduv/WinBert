namespace Arktos.WinBert.Differencing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Arktos.WinBert.Xml;

    /// <summary>
    /// This simple type differencing engine will take two types and figure out the difference between them.
    /// </summary>
    public sealed class TypeDifferenceEngine : IDifferenceEngine<Type, ITypeDifferenceResult>
    {
        #region Fields & Constants

        private readonly IList<IgnoreTarget> ignoreTargets;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the BertTypeDifferenceEngine class.
        /// </summary>
        /// <param name="ignoreTargets">
        /// A list of ignore targets.
        /// </param>
        public TypeDifferenceEngine(IgnoreTarget[] ignoreTargets)
        {
            if (ignoreTargets == null)
            {
                throw new ArgumentNullException("ignoreTargets");
            }

            this.ignoreTargets = ignoreTargets.Where(x => x.Type == IgnoreType.Method).ToList();
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
        /// a hierarchical manner. <see cref="TypeDifferenceResult"/>
        /// </returns>
        public ITypeDifferenceResult Diff(Type oldObject, Type newObject)
        {
            if (oldObject == null)
            {
                throw new ArgumentNullException("oldObject");
            }
            if (newObject == null)
            {
                throw new ArgumentNullException("newObject");
            }

            var diffResult = TypeDifferenceResult.FromType(newObject);
            var oldTypeDict = oldObject.GetMethods().Where(x => x.DeclaringType.Name == oldObject.Name).ToDictionary(x => x.Name);
            var newTypes = newObject.GetMethods().Where(x => x.DeclaringType.Name == newObject.Name && !this.ignoreTargets.Any(y => y.Name.Equals(x.Name)));

            foreach (var method in newTypes)
            {
                if (oldTypeDict.ContainsKey(method.Name))
                {
                    var toCompare = oldTypeDict[method.Name];
                    if (AreDifferent(method.GetMethodBody(), toCompare.GetMethodBody()))
                    {
                        diffResult.Methods.Add(method.Name);
                    }
                }
            }

            return diffResult;
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
            var firstBodyBits = first.GetILAsByteArray();
            var secondBodyBits = second.GetILAsByteArray();

            if (firstBodyBits.Length != secondBodyBits.Length)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < firstBodyBits.Length; i++)
                {
                    if (firstBodyBits[i] != secondBodyBits[i])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}