namespace Arktos.WinBert.Differencing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Arktos.WinBert.Xml;

    /// <summary>
    /// This simple difference engine will take in paths to two assemblies and figure out the difference between them.
    /// </summary>
    public sealed class AssemblyDifferenceEngine : IDifferenceEngine<Assembly, IAssemblyDifferenceResult>
    {
        #region Constants & Fields

        private readonly IList<IgnoreTarget> ignoreTargets = null;
        private readonly TypeDifferenceEngine typeDiffer = null;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the AssemblyDifferenceEngine class.
        /// </summary>
        public AssemblyDifferenceEngine()
            : this(new IgnoreTarget[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the BertAssemblyDifferenceEngine class.
        /// </summary>
        /// <param name="ignoreTargets">
        /// A list of ignore targets.
        /// </param>
        public AssemblyDifferenceEngine(IgnoreTarget[] ignoreTargets)
        {
            if (ignoreTargets == null)
            {
                throw new ArgumentNullException("ignoreTargets");
            }

            this.typeDiffer = new TypeDifferenceEngine(ignoreTargets);
            this.ignoreTargets = ignoreTargets.Where(x => x.Type == IgnoreType.Type).ToList();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Implementation of the Diff method required by the IDifferenceEngine interface. This method will perform
        /// a very rudimentary diff on the two passed in assemblies.
        /// </summary>
        /// <param name="oldObject">
        /// The first object to compare.
        /// </param>
        /// <param name="newObject">
        /// The second object to compare.
        /// </param>
        /// <returns>
        /// An IDifferenceResult implementation that contains all the differences between the target assemblies in
        /// a hierarchical manner.
        /// </returns>
        public IAssemblyDifferenceResult Diff(Assembly oldObject, Assembly newObject)
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
            var oldTypes = oldObject.GetTypes().ToDictionary(x => x.Name);
            var newTypes = newObject.GetTypes().Where(x => !this.ignoreTargets.Any(y => y.Name.Equals(x.FullName))).ToList();

            foreach (var newType in newTypes)
            {
                if (oldTypes.ContainsKey(newType.Name))
                {
                    var oldType = oldTypes[newType.Name];
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