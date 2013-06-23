namespace Arktos.WinBert.Differencing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Arktos.WinBert.Xml;

    /// <summary>
    /// This simple difference engine will take in paths to two assemblies and figure out the difference between them.
    /// This object is remotable into other application domains so as to prevent DLL hell. It *will* need to load both
    /// assemblies into the current application domain in order to properly perform the diff, so it's best to remote this.
    /// </summary>
    public sealed class AssemblyDifferenceEngine : MarshalByRefObject,
        IDifferenceEngine<Assembly, IAssemblyDifferenceResult>
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

        /// <inheritdoc />
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

            int count = 0;
            var typeDiffs = new List<ITypeDifferenceResult>();
            var oldTypes = oldObject.GetTypes().Where(x => !this.ignoreTargets.Any(y => y.Name.Equals(x.FullName)) && !x.IsInterface).ToDictionary(x => x.Name);
            var newTypes = newObject.GetTypes().Where(x => !this.ignoreTargets.Any(y => y.Name.Equals(x.FullName)) && !x.IsInterface).ToList();

            foreach (var newType in newTypes)
            {
                if (oldTypes.ContainsKey(newType.Name))
                {
                    count++;
                    var oldType = oldTypes[newType.Name];
                    var typeDiff = this.typeDiffer.Diff(oldType, newType);

                    if (typeDiff.IsDifferent)
                    {
                        typeDiffs.Add(typeDiff);
                    }
                }
            }

            return new AssemblyDifferenceResult(oldObject, newObject, count, typeDiffs);
        }

        #endregion
    }
}