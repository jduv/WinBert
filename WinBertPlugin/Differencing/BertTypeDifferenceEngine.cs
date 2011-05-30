﻿namespace WinBert.Differencing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using WinBert.Xml;

    /// <summary>
    /// This simple type differencing engine will take two types and figure out the difference between them.
    /// </summary>
    public sealed class BertTypeDifferenceEngine : IDifferenceEngine<Type, ITypeDifferenceResult>
    {
        #region Constants and Fields

        /// <summary>
        ///   A list of ignore targets applicable to methods.
        /// </summary>
        private readonly IgnoreTarget[] ignoreTargets = null;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the BertTypeDifferenceEngine class.
        /// </summary>
        /// <param name="ignoreTargets">
        /// A list of ignore targets.
        /// </param>
        public BertTypeDifferenceEngine(IgnoreTarget[] ignoreTargets)
        {
            if (ignoreTargets != null && ignoreTargets.Length > 0)
            {
                var methodIgnoreTargets = from it in ignoreTargets where it.Type == IgnoreType.Method select it;
                this.ignoreTargets = methodIgnoreTargets.ToArray();
            }
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
        ///   a hierarchical manner. <see cref="TypeDifferenceResult"/>
        /// </returns>
        public ITypeDifferenceResult Diff(Type oldObject, Type newObject)
        {
            TypeDifferenceResult typeDifferenceResult = null;
            if (oldObject == null || newObject == null)
            {
                return null;
            }
            else
            {
                typeDifferenceResult = new TypeDifferenceResult(oldObject, newObject);
                var methodDictionary = this.GetMethodDictionaryForType(oldObject);

                var filteredMethodList = this.GetFilteredMethodListForType(newObject);
                foreach (MethodInfo method in this.GetMethodDifferences(methodDictionary, filteredMethodList))
                {
                    typeDifferenceResult.MethodNames.Add(method);
                }

                return typeDifferenceResult;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns a list of methods filtered based on the ignore targets for this engine.
        /// </summary>
        /// <param name="type">
        /// he type to filter.
        /// </param>
        /// <returns>
        /// A list that meets the method filter criteria.
        /// </returns>
        private IList<MethodInfo> GetFilteredMethodListForType(Type type)
        {
            IEnumerable<MethodInfo> methods = Enumerable.TakeWhile(
                type.GetMethods(), (m) => m.DeclaringType.FullName.Equals(type.FullName));

            if (this.ignoreTargets != null)
            {
                var availableTypes = from m in methods
                                     let ignoreTargets = 
                                     from target in this.ignoreTargets 
                                     select target.Name
                                     where ignoreTargets.Contains(m.Name) == false
                                     select m;

                return Enumerable.ToList(availableTypes);
            }
            else
            {
                return Enumerable.ToArray(methods);
            }
        }

        /// <summary>
        /// Returns a dictionary for the methods of the passed in type. Filters based on declaring type.
        /// </summary>
        /// <param name="type">
        /// The type to build the dictionary for.
        /// </param>
        /// <returns>
        /// A dictionary of the methods for the given type.
        /// </returns>
        private Dictionary<string, MethodInfo> GetMethodDictionaryForType(Type type)
        {
            var methodDictionary = new Dictionary<string, MethodInfo>();

            var filteredMethodList = this.GetFilteredMethodListForType(type);
            foreach (var m in filteredMethodList)
            {
                methodDictionary.Add(m.ToString(), m);
            }

            return methodDictionary;
        }

        /// <summary>
        /// Returns a list of methods that are different based on their MSIL.
        /// </summary>
        /// <param name="methodDictionary">
        /// A type dictionary to check against. This speeds up comparisons quite a bit.
        /// </param>
        /// <param name="methods">
        /// The new type to check.
        /// </param>
        /// <returns>
        /// A list of methods that are different.
        /// </returns>
        private IEnumerable GetMethodDifferences(
            Dictionary<string, MethodInfo> methodDictionary, IList<MethodInfo> methods)
        {
            foreach (var method in methods)
            {
                MethodInfo methodInDictionary = null;
                if (methodDictionary.TryGetValue(method.ToString(), out methodInDictionary))
                {
                    // if the method exists in the dictionary, then compare their method bodies
                    var methodBody = method.GetMethodBody();
                    var methodBodyInDictionary = methodInDictionary.GetMethodBody();

                    if (methodBody != null && methodBodyInDictionary != null)
                    {
                        if (
                            Convert.ToBase64String(methodBody.GetILAsByteArray()).Equals(
                                Convert.ToBase64String(methodBodyInDictionary.GetILAsByteArray())))
                        {
                            // something indeed is different
                            yield return method;
                        }
                    }
                    else
                    {
                        Trace.TraceError(
                            "Error attempting to compare methods {0} and {1}. One of the method bodies is null.", 
                            methodInDictionary, 
                            method);
                    }
                }
            }
        }

        #endregion
    }
}