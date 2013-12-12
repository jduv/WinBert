namespace Arktos.WinBert.Differencing
{
    using Arktos.WinBert.Xml;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// This simple difference engine will take in paths to two assemblies and figure out the difference between them.
    /// This object is remotable into other application domains so as to prevent DLL hell. It *will* need to load both
    /// assemblies into the current application domain in order to properly perform the diff, so it's best to remote this.
    /// </summary>
    public sealed class AssemblyDiffer : MarshalByRefObject,
        IDifferenceEngine<Assembly, IAssemblyDifference>
    {
        #region Fields & Constants

        private readonly IEnumerable<DiffIgnoreTarget> ignoreTargets = null;
        private readonly TypeDiffer typeDiffer = null;

        #endregion

        #region Constructors & Destructors

        public AssemblyDiffer()
            : this(new DiffIgnoreTarget[0])
        {
        }

        public AssemblyDiffer(IEnumerable<DiffIgnoreTarget> ignoreTargets)
        {
            if (ignoreTargets == null)
            {
                throw new ArgumentNullException("ignoreTargets");
            }

            this.typeDiffer = new TypeDiffer(ignoreTargets);
            this.ignoreTargets = ignoreTargets.Where(x => x.Type == DiffIgnoreType.Type).ToList();
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public IAssemblyDifference Diff(Assembly oldObject, Assembly newObject)
        {
            if (oldObject == null)
            {
                throw new ArgumentNullException("oldObject");
            }

            if (newObject == null)
            {
                throw new ArgumentNullException("newObject");
            }

            // Only enumerate public types.
            var typeDiffs = new List<ITypeDifference>();
            var oldTypes = oldObject.GetTypes().Where(x => !this.ignoreTargets.Any(y => y.Name.Equals(x.FullName)) &&
                !x.IsInterface && x.Attributes.HasFlag(TypeAttributes.Public)).ToDictionary(x => x.Name);
            var newTypes = newObject.GetTypes().Where(x => !this.ignoreTargets.Any(y => y.Name.Equals(x.FullName)) &&
                !x.IsInterface && x.Attributes.HasFlag(TypeAttributes.Public)).ToList();

            foreach (var newType in newTypes)
            {
                if (oldTypes.ContainsKey(newType.Name))
                {
                    var oldType = oldTypes[newType.Name];
                    var typeDiff = this.typeDiffer.Diff(oldType, newType);

                    if (typeDiff.AreDifferences)
                    {
                        typeDiffs.Add(typeDiff);
                    }
                }
            }

            return new AssemblyDifference(oldObject, newObject, typeDiffs);
        }

        #endregion
    }
}