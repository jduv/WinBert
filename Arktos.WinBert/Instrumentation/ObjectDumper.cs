namespace Arktos.WinBert.Instrumentation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Arktos.WinBert.Extensions;

    /// <summary>
    /// Logs objects of various types, converting them to a simple XML representation.
    /// </summary>
    public sealed class ObjectDumper
    {
        #region Public Methods

        /// <summary>
        /// In essence, this is a factory method for creating Xml.Object instances. This only works for non-primitive
        /// objects and structs.
        /// </summary>
        /// <param name="target">
        /// The target to dump.
        /// </param>
        /// <param name="maxDepth">
        /// The maximum depth to go when recursively logging instance fields and properties. Defaults to 5.
        /// </param>
        /// <returns>
        /// An Xml.Object instance representing the passed in target.
        /// </returns>
        public Xml.Object DumpObject(object target, ushort maxDepth = 5)
        {
            Xml.Object obj;
            if (target == null)
            {
                obj = new Xml.Null();
            }
            else
            {
                if (maxDepth > 0)
                {
                    if (target.IsPrimitive())
                    {
                        throw new ArgumentException("Cannot dump a primitive type from this method!");
                    }
                    else
                    {
                        obj = new Xml.Object()
                        {
                            Type = target.GetType().FullName,
                            Fields = DumpFields(target, maxDepth),
                            Properties = DumpProperties(target, maxDepth)
                        };
                    }
                }
                else
                {
                    // Target will never be null here, that'll get caught earlier.
                    obj = (Xml.Object)new Xml.NotNull();
                }
            }

            return obj;
        }

        /// <summary>
        /// Creates Xml.Primitive instances. This only works for objects that are considered to be
        /// "primtive," which isn't precisly correlated to the obj.GetType().IsPrimitive property. There
        /// are a couple of other CLR types that can be considered primitives for our purposes here
        /// even if there are no IL instructions that operate on them (which is the CLR's definition of
        /// a primitive as of now).
        /// </summary>
        /// <param name="target">
        /// The target primitive object to dump.
        /// </param>
        /// <returns>
        /// An Xml.Primitive object representing the passed in target.
        /// </returns>
        public Xml.Primitive DumpPrimitive(object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            Xml.Primitive primitive;
            if (target.IsPrimitive())
            {
                primitive = new Xml.Primitive()
                {
                    Type = target.GetType().FullName,
                    Value = target.ToString()
                };
            }
            else
            {
                throw new ArgumentException("Cannot dump non-primitive type from this method!");
            }

            return primitive;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Dumps all the field values for the target object.
        /// </summary>
        /// <param name="target">
        /// The target whose fields to retrieve.
        /// </param>
        /// <param name="maxDepth">
        /// A depth value, used for determining how deep to recursively log field reference
        /// types.
        /// </param>
        /// <returns>
        /// An array of Xml.Field objects.
        /// </returns>
        Xml.Field[] DumpFields(object target, ushort maxDepth)
        {
            var fields = new List<Xml.Field>();
            foreach (var field in target.GetType().GetFields(
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Static))
            {
                var dumpedField = new Xml.Field()
                {
                    Name = field.Name,
                    Value = new Xml.Value()
                };

                var value = field.GetValue(target);
                if (value == null)
                {
                    dumpedField.Value.Item = new Xml.Null();
                }
                else if (value == target)
                {
                    dumpedField.Value.Item = new Xml.This();
                }
                else if (value.IsPrimitive())
                {
                    dumpedField.Value.Item = DumpPrimitive(value);
                }
                else
                {
                    dumpedField.Value.Item = DumpObject(value, (ushort)(maxDepth - 1));
                }

                fields.Add(dumpedField);
            }

            return fields.ToArray();
        }

        /// <summary>
        /// Dumps all the property values for the target object.
        /// </summary>
        /// <param name="target">
        /// The target whose properties to retrieve.
        /// </param>
        /// <param name="maxDepth">
        /// A depth value, used for determining how deep to recursively log property reference
        /// types.
        /// </param>
        /// <returns>
        /// An array of Xml.Property objects.
        /// </returns>
        Xml.Property[] DumpProperties(object target, ushort maxDepth)
        {
            var properties = new List<Xml.Property>();
            foreach (var prop in target.GetType().GetProperties(
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.NonPublic |
                BindingFlags.Public))
            {
                // Only dump properties that are not indexers
                if (prop.GetIndexParameters().Length == 0)
                {
                    var dumpedProperty = new Xml.Property()
                    {
                        Name = prop.Name,
                        Value = new Xml.Value()
                    };

                    var value = prop.GetValue(target, null);
                    if (value == null)
                    {
                        dumpedProperty.Value.Item = new Xml.Null();
                    }
                    else if (value == target)
                    {
                        dumpedProperty.Value.Item = new Xml.This();
                    }
                    else if (value.IsPrimitive())
                    {
                        dumpedProperty.Value.Item = DumpPrimitive(value);
                    }
                    else
                    {
                        dumpedProperty.Value.Item = DumpObject(value, (ushort)(maxDepth - 1));
                    }

                    properties.Add(dumpedProperty);
                }
            }

            return properties.ToArray();
        }

        #endregion
    }
}
