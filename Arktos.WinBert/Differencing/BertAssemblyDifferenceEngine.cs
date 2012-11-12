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
    public sealed class BertAssemblyDifferenceEngine : IDifferenceEngine<Assembly, AssemblyDifferenceResult>
    {
        #region Constants and Fields

        /// <summary>
        ///   A list of ignore targets applicable to types.
        /// </summary>
        private readonly IgnoreTarget[] ignoreTargets = null;

        /// <summary>
        ///   A mechanism for differencing types.
        /// </summary>
        private readonly BertTypeDifferenceEngine typeDiffer = null;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the BertAssemblyDifferenceEngine class.
        /// </summary>
        /// <param name="ignoreTargets">
        /// A list of ignore targets.
        /// </param>
        public BertAssemblyDifferenceEngine(IgnoreTarget[] ignoreTargets)
        {
            this.typeDiffer = new BertTypeDifferenceEngine(ignoreTargets);

            if (ignoreTargets != null && ignoreTargets.Length > 0)
            {
                this.ignoreTargets = (from it in ignoreTargets where it.Type == IgnoreType.Type select it).ToArray();                
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Implementation of the Diff method required by the IDifferenceEngine interface. This method will perform
        ///   a very rudimentary diff on the two passed in assemblies.
        /// </summary>
        /// <param name="oldObject">
        /// The first object to compare.
        /// </param>
        /// <param name="newObject">
        /// The second object to compare.
        /// </param>
        /// <returns>
        /// An IDifferenceResult implementation that contains all the differences between the target assemblies in
        ///   a hierarchical manner.
        /// </returns>
        public AssemblyDifferenceResult Diff(Assembly oldObject, Assembly newObject)
        {
            AssemblyDifferenceResult assemblyDifferenceResult = null;
            if (oldObject == null || newObject == null)
            {
                return null;
            }
            else
            {
                assemblyDifferenceResult = new AssemblyDifferenceResult(oldObject, newObject);
                var typeDictionary = this.GetTypeDictionaryForAssembly(oldObject);

                // dictionary is filtered in GetTypeDictionaryForAssembly, filter our new type's list for consistency
                var filteredTypeList = this.GetFilteredTypeList(newObject.GetTypes());
                foreach (Type newType in filteredTypeList)
                {
                    if (typeDictionary.ContainsKey(newType.FullName))
                    {
                        var oldType = typeDictionary[newType.FullName];
                        var typeDifference = this.Diff(oldType, newType);

                        if (typeDifference != null && typeDifference.IsDifferent)
                        {
                            assemblyDifferenceResult.TypeDifferences.Add(typeDifference);
                        }
                    }
                }

                return assemblyDifferenceResult;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the difference between the two passed in types as a TypeDifferenceResult.
        /// </summary>
        /// <param name="oldType">
        /// The first type to diff
        /// </param>
        /// <param name="newType">
        /// The second type to diff
        /// </param>
        /// <returns>
        /// An IDifferenceResult implementation that contains all the differences between the target types in
        ///   a hierarchical manner. <see cref="TypeDifferenceResult"/>
        /// </returns>
        private ITypeDifferenceResult Diff(Type oldType, Type newType)
        {
            return this.typeDiffer.Diff(oldType, newType);
        }

        /// <summary>
        /// Returns a list of types filtered based on the ignore targets for this engine.
        /// </summary>
        /// <param name="types">
        /// The list of types to filter
        /// </param>
        /// <returns>
        /// A list of types that meet the filter criteria.
        /// </returns>
        private IList<Type> GetFilteredTypeList(Type[] types)
        {
            if (this.ignoreTargets != null)
            {
                var availableTypes = from t in types
                                                   let ignoreTargets = 
                                                   from target in this.ignoreTargets 
                                                   select target.Name
                                                   where ignoreTargets.Contains(t.FullName) == false
                                                   select t;

                return availableTypes.ToList();
            }
            else
            {
                return types;
            }
        }

        /// <summary>
        /// Returns a dictionary containing a list of all the types in the target assembly. Since there can be, by 
        ///   definition, only one type of a certain name in existence inside an assembly, this method should never 
        ///   run into collisions. Note that this method will also filter out any unwanted types based on the ignore 
        ///   targets for this engine.
        /// </summary>
        /// <param name="assembly">
        /// The assembly to enumerate all the types for.
        /// </param>
        /// <returns>
        /// A dictionary containing references to all the types in the assembly with their names as keys. If no types
        ///   exist in the assembly (unlikely) then an empty dictionary will be returned.
        /// </returns>
        private Dictionary<string, Type> GetTypeDictionaryForAssembly(Assembly assembly)
        {
            return this.GetFilteredTypeList(assembly.GetTypes()).ToDictionary(x => x.FullName);
        }

        #endregion
    }
}