namespace Arktos.WinBert.Differencing.Cci
{
    using System;
    using System.Linq;
    using Microsoft.Cci;
    using Arktos.WinBert.Xml;
    using System.Collections.Generic;

    /// <summary>
    /// A difference engine based on the CCI metadata API.
    /// </summary>
    public class CciAssemblyDifferenceEngine : IDifferenceEngine<IAssembly, ICciAssemblyDifferenceResult>
    {
        #region Fields & Constants

        private readonly IList<IgnoreTarget> ignoreTargets;
        private readonly CciTypeDifferenceEngine typeDiffer;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// /// Initializes a new instance of the CciAssemblyDifferenceEngine class.
        /// </summary>
        public CciAssemblyDifferenceEngine() : this(new IgnoreTarget[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the CciAssemblyDifferenceEngine class.
        /// </summary>
        /// <param name="ignoreTargets">
        /// A list of ignore targets.
        /// </param>
        public CciAssemblyDifferenceEngine(IgnoreTarget[] ignoreTargets)
        {
            if (ignoreTargets == null)
            {
                throw new ArgumentNullException("ignoreTargets");
            }

            this.typeDiffer = new CciTypeDifferenceEngine(ignoreTargets);
            this.ignoreTargets = ignoreTargets.Where(x => x.Type == IgnoreType.Type).ToList();
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public ICciAssemblyDifferenceResult Diff(IAssembly oldObject, IAssembly newObject)
        {
            if (oldObject == null)
            {
                throw new ArgumentNullException("oldObject");
            }

            if (newObject == null)
            {
                throw new ArgumentNullException("newObject");
            }

            var diffResult = new CciAssemblyDifferenceResult(oldObject, newObject);
            var oldTypesDict = oldObject.GetAllTypes().ToDictionary(x => x.Name);
            var newTypes = newObject.GetAllTypes().Where(x => this.ignoreTargets.Any(y => y.Name.Equals(y.Name)));

            foreach (var newType in newTypes)
            {
                if (oldTypesDict.ContainsKey(newType.Name))
                {
                    var oldType = oldTypesDict[newType.Name];
                    var typeDiff = this.typeDiffer.Diff(oldType, newType);

                    if (typeDiff.IsDifferent)
                    {
                        diffResult.TypeDifferences.Add(typeDiff);
                    }
                }
            }

            return diffResult;
        }

        #endregion
    }
}
