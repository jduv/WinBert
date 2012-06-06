//*********************************************************
//
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the Apache License, Version 2.0.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************



using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Common;

namespace Randoop
{
    public interface IReflectionFilter
    {
        bool OkToUse(FieldInfo f, out string message);
        bool OkToUse(MethodInfo m, out string message);
        bool OkToUse(ConstructorInfo c, out string mssage);
        bool OkToUse(Type c, out string message);

    }

    /// <summary>
    /// A filter that is composed of other filters
    /// Interpreted as the intersection of filters
    /// </summary>
    public class ComposableFilter : IReflectionFilter
    {
        Collection<IReflectionFilter> filters = new Collection<IReflectionFilter>();

        public ComposableFilter(params IReflectionFilter[] f)
        {
            foreach (IReflectionFilter filter in f)
                filters.Add(filter);
        }

        public void Add(IReflectionFilter f)
        {
            filters.Add(f);

        }

        public bool OkToUse(FieldInfo f, out string message)
        {
            message = null;
            foreach (IReflectionFilter filter in filters)
                if (!filter.OkToUse(f, out message))
                    return false;
            return true;
        }

        public bool OkToUse(MethodInfo m, out string message)
        {
            message = null;
            foreach (IReflectionFilter filter in filters)
                if (!filter.OkToUse(m, out message))
                    return false;
            return true;
        }

        public bool OkToUse(ConstructorInfo c, out string message)
        {
            message = null;
            foreach (IReflectionFilter filter in filters)
                if (!filter.OkToUse(c, out message))
                    return false;
            return true;
        }

        public bool OkToUse(Type t, out string message)
        {
            message = null;
            foreach (IReflectionFilter filter in filters)
                if (!filter.OkToUse(t, out message))
                    return false;
            return true;
        }

    }

    /// <summary>
    /// This filter can be created with two different behaviors.
    /// If created with useInternal==false, then only public members are ok.
    /// This is the filter to modify if we want to prevent something from being explored
    /// </summary>
    public class VisibilityFilter : IReflectionFilter
    {
        public bool useStaticMethods;
        public bool useInternal;

        public VisibilityFilter(RandoopConfiguration env)
        {
            this.useStaticMethods = env.usestatic;
            this.useInternal = env.useinternal;
        }

        public VisibilityFilter(bool useStatic, bool useInternal)
        {
            this.useStaticMethods = useStatic;
            this.useInternal = useInternal;
        }

        public bool OkToUse(FieldInfo fi, out string message)
        {
            //Console.Write("FFF" + fi.ToString() + " ");
            if (!fi.IsPublic)
            {
                //Console.WriteLine("FALSE");
                message = "Will not use: field is not public: " + fi.ToString();
                return false;
            }

            // Since we only currently use fields for FieldSettingTransformers,
            // we do not want any fields that cannot be modified.
            if (fi.IsInitOnly)
            {
                message = "Will not use: field is readonly: " + fi.ToString();
                return false;
            }

            if (fi.IsLiteral)
            {
                message = "Will not use: field is const: " + fi.ToString();
                return false;
            }

            // We probably don't want to modify static fields because
            // our notion of plans does not account for global state.
            if (fi.IsStatic)
            {
                //Console.WriteLine("FALSE");
                message = "Will not use: field is static: " + fi.ToString();
                return false;
            }

            //Console.WriteLine("TRUE");
            message = "@@@OK6" + fi.ToString();
            return true;
        }

        public bool OkToUse(Type t, out string message)
        {
            // The only purpose of including an interface would be to
            // add the methods they declare to the list of methods to
            // explore. But this is not necessary because classes that
            // implement the interface will declare these methods, so
            // we'll get the methods when we process the implementing
            // classes. Also, a method may implement an interface but
            // declare the implemented methods private (for example).
            if (t.IsInterface)
            {
                message = "Will not use: type is interface: " + t.ToString();
                return false;
            }

            if (ReflectionUtils.IsSubclassOrEqual(t, typeof(System.Delegate)))
            {
                message = "Will not use: type is delegate: " + t.ToString();
                return false;
            }

            if (!useInternal && !t.IsPublic)
            {
                message = "Will not use: type is no public: " + t.ToString();
                return false;
            }

            if (t.IsNested)
            {
                if (t.IsNestedPrivate)
                {
                    message = "Will not use: type is nested private: " + t.ToString();
                    return false;
                }
            }
            else
            {
                if (t.IsNotPublic)
                {
                    message = "Will not use: type is not public: " + t.ToString();
                    return false;
                }
            }
            message = "@@@OK7" + t.ToString();
            return true;

        }

        public bool OkToUse(ConstructorInfo c, out string message)
        {

            if (c.DeclaringType.IsAbstract)
            {
                message = "Will not use: constructor's declaring type is abstract: " + c.ToString();
                return false;
            }

            return OkToUseBase(c, out message);
        }

        public bool OkToUse(MethodInfo m, out string message)
        {
            return OkToUseBase(m, out message);
        }

        /// <summary>
        /// TODO: Resolve IsPublic, IsNotPublic semantics of reflection
        /// </summary>
        /// <param name="ci"></param>
        /// <returns></returns>
        private bool OkToUseBase(MethodBase ci, out string message)
        {
            if (ci.IsAbstract)
            {
                message = "Will not use: method or constructor is abstract: " + ci.ToString();
                return false;
            }

            foreach (ParameterInfo pi in ci.GetParameters())
            {
                if (pi.ParameterType.Name.EndsWith("&"))
                {
                    message = "Will not use: method or constructor has a parameter containing \"&\": " + ci.ToString();
                    return false;
                }
                if (pi.IsOut)
                {
                    message = "Will not use: method or constructor has an out parameter: " + ci.ToString();
                    return false;
                }
                if (pi.ParameterType.IsGenericParameter)
                {
                    message = "Will not use: method or constructor has a generic parameter: " + ci.ToString();
                    return false;
                }
            }

            if (!this.useInternal && !ci.IsPublic)
            {
                message = "Will not use: method or constructor is not public: " + ci.ToString();
                return false;
            }

            if (ci.IsPrivate)
            {
                message = "Will not use: method or constructor is private: " + ci.ToString();
                return false;
            }

            if (ci.IsStatic)
            {
                if (!useStaticMethods)
                {
                    message = "Will not use: method or constructor is static: " + ci.ToString();
                    return false;
                }
            }

            if (ci.DeclaringType.Equals(typeof(object)))
            {
                message = "Will not use: method is System.Object's: " + ci.ToString();
                return false;
            }


            foreach (Attribute attr in ci.GetCustomAttributes(true))
            {

                if (attr is ObsoleteAttribute)
                {
                    //WriteLine("Obsolete Method " + ci.DeclaringType + "::" + ci.Name + "()" + "detected\n");
                    message = "Will not use: has attribute System.ObsoleteAttribute: " + ci.ToString();
                    return false;
                }
                //TODO: there are still cases where an obsolete method is not caught. e.g. System.Xml.XmlSchema::ElementType

            }

            message = "@@@OK8" + ci.ToString();
            return true;
        }
    }

}
