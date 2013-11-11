namespace Arktos.WinBert.Instrumentation
{
    using System.Linq;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Arktos.WinBert.Extensions;
    using Arktos.WinBert.Xml;

    /// <summary>
    /// Logs objects of various types, converting them to a simple XML representation.
    /// </summary>
    public sealed class ObjectDumper : IObjectDumper
    {
        #region Fields & Constants

        private readonly HashSet<string> ignoreTargetLookup;

        #endregion

        #region Constructors & Destructors

        public ObjectDumper()
            : this(null)
        {
        }

        public ObjectDumper(IEnumerable<DumpIgnoreTarget> ignoreTargets)
        {
            this.ignoreTargetLookup = new HashSet<string>();
            if (ignoreTargets != null)
            {
                // This looks quite ugly but it simply concatenates the fully qualified type name with the field and property
                // names stored inside the ignore type objects. This is then added ot the hash set for O(1) lookups later.
                var toIgnore = ignoreTargets.SelectMany(i => i.FieldAndPropertyNames == null ? new string[0] : i.FieldAndPropertyNames,
                    (t, n) => string.Join(".", t.Type, n));
                foreach (var item in toIgnore)
                {
                    ignoreTargetLookup.Add(item);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public Xml.Object DumpObject(object target, ushort maxDepth = 3)
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
                        var fieldsAndProps = DumpFieldsAndProperties(target, maxDepth);
                        obj = new Xml.Object()
                        {
                            Type = target.GetType().FullName,
                            Fields = fieldsAndProps.Fields,
                            AutoProperties = fieldsAndProps.Properties
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

        /// <inheritdoc />
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
        /// Dump all fields and properties of the target object up to the maximum supplied depth. Note that
        /// only auto-generated properties will be dumped, and the fields corresponding to those properties
        /// will be pushed into the property as the backing field element.
        /// </summary>
        /// <param name="target">
        /// The target to dump.
        /// </param>
        /// <param name="maxDepth">
        /// The maximum depth to dump.
        /// </param>
        /// <returns>
        /// A FieldsAndProperties class containing the dumped fields and properties.
        /// </returns>
        private FieldsAndProperties DumpFieldsAndProperties(object target, ushort maxDepth)
        {
            var dumpedFields = new List<Xml.Field>();
            var dumpedProps = new List<Xml.Property>();
            var compilerGeneratedFields = new List<FieldInfo>();

            // First, spin through the fields.
            foreach (var field in target.GetType().GetFields(
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Static)
                .Where(x => !this.IsTargetIgnored(x.DeclaringType.FullName, x.Name)))
            {
                if (!(field.IsCompilerGenerated() || target.CouldBeAnonymousType()))
                {
                    dumpedFields.Add(DumpField(target, field, maxDepth));
                }
                else
                {
                    compilerGeneratedFields.Add(field);
                }
            }

            // Next, spin through the properties, looking for only auto generated
            // ones. Match the compiler generated field with the property.
            foreach (var prop in target.GetType().GetProperties(
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.NonPublic |
                BindingFlags.Public))
            {
                if (prop.GetIndexParameters().Length == 0 && (target.CouldBeAnonymousType() || prop.MightBeAutoGenerated()))
                {
                    // Look for backing field.
                    var backingField = FindBackingField(target, compilerGeneratedFields, prop);
                    if (backingField != null)
                    {
                        // Don't dump the field if it's ignored.
                        if (!this.IsTargetIgnored(prop.DeclaringType.FullName, prop.Name))
                        {
                            dumpedProps.Add(DumpProperty(target, backingField, prop, maxDepth));
                        }

                        // Remove the field no matter what, else dumped values might not line up.
                        compilerGeneratedFields.Remove(backingField);
                    }
                }
            }

            // Process what's left of the backing fields list--just in case something didn't line up with a property.
            foreach (var leftoverField in compilerGeneratedFields)
            {
                dumpedFields.Add(DumpField(target, leftoverField, maxDepth));
            }

            return new FieldsAndProperties(dumpedFields, dumpedProps);
        }

        /// <summary>
        /// Finds the backing field for the target property.
        /// </summary>
        /// <param name="target">
        /// The target object.
        /// </param>
        /// <param name="compilerGeneratedFields">
        /// The list of fields to search.
        /// </param>
        /// <param name="prop">
        /// The property whose backing field to find.
        /// </param>
        /// <returns>
        /// The backing field.
        /// </returns>
        private static FieldInfo FindBackingField(object target, List<FieldInfo> compilerGeneratedFields, PropertyInfo prop)
        {
            return compilerGeneratedFields.FirstOrDefault(
                (x) =>
                {
                    return x.Name.StartsWith("<" + prop.Name + ">") &&
                        ((target.CouldBeAnonymousType() && x.Name.Contains(Members.AnonymousTypeFieldDesignation)) ||
                        x.Name.Contains(Members.BackingFieldDesignation));
                });
        }

        /// <summary>
        /// Dumps the target field and any containing members up to the supplied depth.
        /// </summary>
        /// <param name="target">
        /// The target object.
        /// </param>
        /// <param name="field">
        /// The field to dump.
        /// </param>
        /// <param name="maxDepth">
        /// The maximum depth to traverse members.
        /// </param>
        /// <returns>
        /// The dumped field.
        /// </returns>
        private Xml.Field DumpField(object target, FieldInfo field, ushort maxDepth)
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

            return dumpedField;
        }

        /// <summary>
        /// Dumps the target field and any containing members up to the supplied depth.
        /// </summary>
        /// <param name="target">
        /// The target object.
        /// </param>
        /// <param name="backingField">
        /// The backing field of the target property.
        /// </param>
        /// <param name="prop">
        /// The property to dump.
        /// </param>
        /// <param name="maxDepth">
        /// The maximum depth to traverse members.
        /// </param>
        /// <returns>
        /// The dumped property.
        /// </returns>
        private Xml.Property DumpProperty(object target, FieldInfo backingField, PropertyInfo prop, ushort maxDepth)
        {
            var dumpedProperty = new Xml.Property()
            {
                Name = prop.Name,
                Value = new Xml.Value(),
                BackingField = DumpField(target, backingField, (ushort)(maxDepth - 1))
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

            return dumpedProperty;
        }

        /// <summary>
        /// Checks to see if the target type and field or property name exists in the
        /// ignore targets list.
        /// </summary>
        /// <param name="type">
        /// The type name.
        /// </param>
        /// <param name="name">
        /// the field or property name.
        /// </param>
        /// <returns>
        /// True if the ignore type is in the lookup, false otherwise.
        /// </returns>
        private bool IsTargetIgnored(string type, string name)
        {
            var qualifiedMember = string.Join(".", type, name);
            return this.ignoreTargetLookup.Contains(qualifiedMember);
        }

        #endregion

        #region Private Classes

        /// <summary>
        /// Simple inner class to represent a combined return value of the field/property dumping
        /// algorithm. The need for this stemmed from a combining of the logic to generate both lists
        /// for performance reasons.
        /// </summary>
        private class FieldsAndProperties
        {
            #region Constructors & Destructors

            public FieldsAndProperties(List<Xml.Field> fields, List<Xml.Property> properties)
            {
                if (fields == null || properties == null)
                {
                    throw new ArgumentNullException(fields == null ? "fields" : "properties");
                }

                this.Fields = fields;
                this.Properties = properties;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets a list of fields.
            /// </summary>
            public List<Xml.Field> Fields { get; private set; }

            /// <summary>
            /// Gets the list of properties.
            /// </summary>
            public List<Xml.Property> Properties { get; private set; }

            #endregion
        }

        #endregion
    }
}
