﻿namespace Arktos.WinBert.Differencing.Cci
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Arktos.WinBert.Xml;
    using Microsoft.Cci;

    /// <summary>
    /// A difference engine based on the CCI metadata API.
    /// </summary>
    public class CciAssemblyDifferenceEngine : IDifferenceEngine<IAssembly, IAssemblyDifferenceResult>
    {
        #region Fields & Constants

        private readonly IList<IgnoreTarget> ignoreTargets;
        private readonly CciTypeDifferenceEngine typeDiffer;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// /// Initializes a new instance of the CciAssemblyDifferenceEngine class.
        /// </summary>
        public CciAssemblyDifferenceEngine()
            : this(new IgnoreTarget[0])
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
        public IAssemblyDifferenceResult Diff(IAssembly oldObject, IAssembly newObject)
        {
            if (oldObject == null)
            {
                throw new ArgumentNullException("oldObject");
            }

            if (newObject == null)
            {
                throw new ArgumentNullException("newObject");
            }

            var diffResult = AssemblyDifferenceResult.Create(oldObject, newObject);
            var oldTypes = oldObject.GetAllTypes().ToDictionary(x => x.Name.Value);
            var newTypes = newObject.GetAllTypes().Where(x => !this.ignoreTargets.Any(y => y.Name.Equals(y.Name)));

            foreach (var newType in newTypes)
            {
                if (oldTypes.ContainsKey(newType.Name.Value))
                {
                    var oldType = oldTypes[newType.Name.Value];
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
