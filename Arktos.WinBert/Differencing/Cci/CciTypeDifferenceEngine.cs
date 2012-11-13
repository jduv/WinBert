namespace Arktos.WinBert.Differencing.Cci
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Arktos.WinBert.Xml;
    using Microsoft.Cci;

    /// <summary>
    /// Performs type differences in a CCI metadata context.
    /// </summary>
    public class CciTypeDifferenceEngine : IDifferenceEngine<INamedTypeDefinition, ICciTypeDifferenceResult>
    {
        #region Fields & Constants

        private readonly IList<IgnoreTarget> ignoreTargets;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the CciTypeDifferenceEngine class.
        /// </summary>
        /// <param name="ignoreTargets">
        /// A list of ignore targets.
        /// </param>
        public CciTypeDifferenceEngine(IgnoreTarget[] ignoreTargets)
        {
            if (ignoreTargets == null)
            {
                throw new ArgumentNullException("ignoreTargets");
            }

            this.ignoreTargets = ignoreTargets.Where(x => x.Type == IgnoreType.Method).ToList();
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public ICciTypeDifferenceResult Diff(INamedTypeDefinition oldObject, INamedTypeDefinition newObject)
        {
            if (oldObject == null)
            {
                throw new ArgumentNullException("oldObject");
            }
            if (newObject == null)
            {
                throw new ArgumentNullException("newObject");
            }

            var diffResult = new CciTypeDifferenceResult(oldObject, newObject);
            var oldTypeDict = oldObject.Methods.ToDictionary(x => x.Name.Value);
            var newTypes = newObject.Methods.Where(x => !this.ignoreTargets.Any(y => y.Name.Equals(x.Name)));

            foreach (var method in newTypes)
            {
                if (oldTypeDict.ContainsKey(method.Name.Value))
                {
                    var toCompare = oldTypeDict[method.Name.Value];
                    if (AreDifferent(method.Body, toCompare.Body))
                    {
                        diffResult.Methods.Add(method);
                    }
                }
            }

            return diffResult;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines if the two method bodies are equal.
        /// </summary>
        /// <param name="first">
        /// The first method body.
        /// </param>
        /// <param name="second">
        /// The second method body.
        /// </param>
        /// <returns>
        /// True if both method bodies are equal, false otherwise.
        /// </returns>
        private bool AreDifferent(IMethodBody first, IMethodBody second)
        {
            bool different = false;
            if (first.Size != second.Size)
            {
                different = true;
            }
            else
            {
                different = this.CompareOperations(first.Operations.ToList(), second.Operations.ToList());
            }

            return different;
        }

        /// <summary>
        /// Compares the two target lists of operations.
        /// </summary>
        /// <param name="firstList">
        /// The first list.
        /// </param>
        /// <param name="secondList">
        /// The second list.
        /// </param>
        /// <returns>
        /// True if the operations lists are the same, false otherwise.
        /// </returns>
        private bool CompareOperations(IList<IOperation> firstList, IList<IOperation> secondList)
        {
            bool different = false;
            if (firstList.Count != secondList.Count)
            {
                different = true;
            }
            else
            {
                int i = 0;
                IOperation first, second;
                while (!different && i < firstList.Count)
                {
                    first = firstList[i];
                    second = secondList[i];

                    // Use this to break the loop.
                    if (first.Offset != second.Offset ||
                        first.OperationCode != second.OperationCode ||
                        first.Value != second.Value)
                    {
                        different = true;
                    }

                    // Increment.
                    i++;
                }
            }

            return different;
        }

        #endregion
    }
}
