namespace Arktos.WinBert.Instrumentation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Arktos.WinBert.Extensions;

    /// <summary>
    /// Logs objects of various types, converting them to a simple XML representation.
    /// </summary>
    public class ObjectDumper
    {
        #region Public Methods

        public Xml.Object DumpObject(object target, ushort maxDepth = 10)
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
                    obj = (target == null) ? (Xml.Object)new Xml.Null() : (Xml.Object)new Xml.NotNull();
                }
            }

            return obj;
        }

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
                else if (maxDepth <= 1)
                {
                    dumpedField.Value.Item = new Xml.NotNull();
                }
                else
                {
                    dumpedField.Value.Item = DumpObject(value, (ushort)(maxDepth - 1));
                }

                fields.Add(dumpedField);
            }

            return fields.ToArray();
        }

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
                    else if (maxDepth <= 1)
                    {
                        dumpedProperty.Value.Item = new Xml.NotNull();
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
