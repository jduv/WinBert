namespace Arktos.WinBert.UnitTests
{
    using Arktos.WinBert.Instrumentation;
    using Arktos.WinBert.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml;
    using System.Runtime.CompilerServices;

    [TestClass]
    public class ObjectDumperUnitTests
    {
        #region Fields & Constants

        private static readonly string NullDump = @"<Null xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://arktos.org/WinBertAnalysisLogSchema.xsd"" />";

        private static readonly string NotNullDump = @"<NotNull xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://arktos.org/WinBertAnalysisLogSchema.xsd"" />";

        #endregion

        #region Test Methods

        #region DumpObject

        [TestMethod]
        public void DumpObject_NullArgument()
        {
            var target = new ObjectDumper();
            var actual = target.DumpObject(null);
            Assert.AreEqual(NullDump, Serializer.XmlSerialize(actual, new XmlWriterSettings() { OmitXmlDeclaration = true }));
        }

        [TestMethod]
        public void DumpObject_NullArgument_ZeroDepth()
        {
            var target = new ObjectDumper();
            var actual = target.DumpObject(null, maxDepth: 0);
            Assert.AreEqual(NullDump, Serializer.XmlSerialize(actual, new XmlWriterSettings() { OmitXmlDeclaration = true }));
        }

        [TestMethod]
        public void DumpObject_NonNullArgument_ZeroDepth()
        {
            var target = new ObjectDumper();
            var actual = target.DumpObject(new { x = 1, y = 2 }, maxDepth: 0);
            Assert.AreEqual(NotNullDump, Serializer.XmlSerialize(actual, new XmlWriterSettings() { OmitXmlDeclaration = true }));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DumpObject_AttemptPrimitiveType()
        {
            var target = new ObjectDumper();
            var actual = target.DumpObject(5);
        }

        [TestMethod]
        public void DumpObject_DateTime()
        {
            var now = DateTime.Now;
            var target = new ObjectDumper();
            var actual = target.DumpObject(now);
            Assert.AreEqual(now.GetType().FullName, actual.Type);
        }

        [TestMethod]
        public void DumpObject_ValueType()
        {
            var toDump = new TestPoint();
            var target = new ObjectDumper();
            var actual = target.DumpObject(toDump);

            Assert.AreEqual(toDump.GetType().FullName, actual.Type);
            Assert.AreEqual(2, actual.Fields.Count);
            Assert.AreEqual(0, actual.AutoProperties.Count);

            // Fields
            var xFieldValue = actual.Fields[0].Value.Item as Xml.Primitive;
            var yFieldValue = actual.Fields[0].Value.Item as Xml.Primitive;
            Assert.IsNotNull(xFieldValue);
            Assert.IsNotNull(yFieldValue);

            Assert.AreEqual("x", actual.Fields[0].Name);
            Assert.AreEqual(toDump.X.GetType().FullName, xFieldValue.Type);
            Assert.AreEqual(toDump.X.ToString(), xFieldValue.Value);

            Assert.AreEqual("y", actual.Fields[1].Name);
            Assert.AreEqual(toDump.Y.GetType().FullName, yFieldValue.Type);
            Assert.AreEqual(toDump.Y.ToString(), yFieldValue.Value);
        }

        [TestMethod]
        public void DumpObject_SimpleObjectWithFields()
        {
            var toDump = new AllPrimitiveFields();
            var target = new ObjectDumper();
            var actual = target.DumpObject(toDump);
            Assert.AreEqual(toDump.GetType().FullName, actual.Type);
            Assert.IsNotNull(actual.Fields);
            Assert.IsNotNull(actual.AutoProperties);
            Assert.AreEqual(12, actual.Fields.Count);
            Assert.AreEqual(0, actual.AutoProperties.Count);
        }

        [TestMethod]
        public void DumpObject_SimpleObjectWithProperties()
        {
            var toDump = new AllProperties();
            var target = new ObjectDumper();
            var actual = target.DumpObject(toDump);

            Assert.AreEqual(toDump.GetType().FullName, actual.Type);
            Assert.IsNotNull(actual.Fields);
            Assert.IsNotNull(actual.AutoProperties);
            Assert.AreEqual(0, actual.Fields.Count);
            Assert.AreEqual(12, actual.AutoProperties.Count);
        }

        [TestMethod]
        public void DumpObject_ObjectWithFieldsAndProperties()
        {
            var toDump = new FieldsAndProperties();
            var target = new ObjectDumper();
            var actual = target.DumpObject(toDump);

            Assert.AreEqual(toDump.GetType().FullName, actual.Type);
            Assert.IsNotNull(actual.Fields);
            Assert.IsNotNull(actual.AutoProperties);
            Assert.AreEqual(6, actual.Fields.Count);
            Assert.AreEqual(6, actual.AutoProperties.Count);
        }

        [TestMethod]
        public void DumpObject_ComplexAnonymousObjectDepthOfOne()
        {
            var toDump = new { x = 1, y = new { a = 2, b = 3, c = 4 }, z = (string)null };
            var target = new ObjectDumper();
            var actual = target.DumpObject(toDump, 1);
            Assert.AreEqual(toDump.GetType().FullName, actual.Type);
            Assert.IsNotNull(actual.Fields);
            Assert.IsNotNull(actual.AutoProperties);

            Assert.AreEqual(0, actual.Fields.Count);
            Assert.AreEqual(3, actual.AutoProperties.Count);

            Assert.IsNotNull(actual.AutoProperties[0]);
            Assert.IsNotNull(actual.AutoProperties[1]);
            Assert.IsNotNull(actual.AutoProperties[2]);

            // Check first field/prop
            var xPropValue = actual.AutoProperties[0].Value.Item as Xml.Primitive;
            Assert.IsNotNull(xPropValue);
            Assert.AreEqual(toDump.x.ToString(), xPropValue.Value);

            // Check second field/prop
            var yPropValue = actual.AutoProperties[1].Value.Item as Xml.NotNull;
            Assert.IsNotNull(yPropValue);

            // Check last field/prop
            var zPropValue = actual.AutoProperties[2].Value.Item as Xml.Null;
            Assert.IsNotNull(zPropValue);
        }

        [TestMethod]
        public void DumpObject_SelfReference()
        {
            var toDump = new SelfReference();
            var target = new ObjectDumper();
            var actual = target.DumpObject(toDump);

            Assert.IsNotNull(actual);
            Assert.AreEqual(toDump.GetType().FullName, actual.Type);

            Assert.AreEqual(1, actual.Fields.Count);
            Assert.AreEqual(0, actual.AutoProperties.Count);

            var refValue = actual.Fields[0].Value.Item as Xml.This;
            Assert.AreEqual("reference", actual.Fields[0].Name);
            Assert.IsNotNull(refValue);
        }

        [TestMethod]
        public void DumpObject_NullIgnoreTargets()
        {
            var toDump = new FieldsAndProperties();
            var target = new ObjectDumper();
            target.IgnoreTargets = null;

            var actual = target.DumpObject(toDump);
            Assert.IsNotNull(actual);
            Assert.AreEqual(toDump.GetType().FullName, actual.Type);

            Assert.AreEqual(6, actual.Fields.Count);
            Assert.AreEqual(6, actual.AutoProperties.Count);
        }

        [TestMethod]
        public void DumpObject_EmptyNamesIgnoreTarget()
        {
            var toDump = new FieldsAndProperties();
            var target = new ObjectDumper();

            var toIgnore = new DumpIgnoreTarget()
            {
                Type = toDump.GetType().FullName,
                FieldAndPropertyNames = null,
            };

            target.IgnoreTargets = new List<DumpIgnoreTarget> { toIgnore };

            var actual = target.DumpObject(toDump);
            Assert.IsNotNull(actual);
            Assert.AreEqual(toDump.GetType().FullName, actual.Type);

            Assert.AreEqual(6, actual.Fields.Count);
            Assert.AreEqual(6, actual.AutoProperties.Count);
        }

        [TestMethod]
        public void DumpObject_NoHitIgnoreTargets()
        {
            var toDump = new FieldsAndProperties();
            var target = new ObjectDumper();

            var toIgnore = new DumpIgnoreTarget()
            {
                Type = toDump.GetType().FullName,
                FieldAndPropertyNames = new string[] { "bippity", "boppity", "boo" },
            };

            target.IgnoreTargets = new List<DumpIgnoreTarget> { toIgnore };

            var actual = target.DumpObject(toDump);
            Assert.IsNotNull(actual);
            Assert.AreEqual(toDump.GetType().FullName, actual.Type);

            Assert.AreEqual(6, actual.Fields.Count);
            Assert.AreEqual(6, actual.AutoProperties.Count);
        }

        [TestMethod]
        public void DumpObject_WithIgnoreTargets()
        {
            var toDump = new FieldsAndProperties();
            var target = new ObjectDumper();

            var toIgnore = new DumpIgnoreTarget()
            {
                Type = toDump.GetType().FullName,
                FieldAndPropertyNames = new string[] { "boolField", "unsignedLongField", "IntProp" },
            };

            target.IgnoreTargets = new List<DumpIgnoreTarget> { toIgnore };

            var actual = target.DumpObject(toDump);
            Assert.IsNotNull(actual);
            Assert.AreEqual(toDump.GetType().FullName, actual.Type);

            Assert.AreEqual(4, actual.Fields.Count);
            Assert.AreEqual(5, actual.AutoProperties.Count);
        }

        [TestMethod]
        public void DumpObject_AnonymousTypeWithIgnoreTargets()
        {
            var toDump = new { X = 1, Y = 2, Z = 3 };
            var target = new ObjectDumper();

            var toIgnore = new DumpIgnoreTarget()
            {
                Type = toDump.GetType().FullName,
                FieldAndPropertyNames = new string[] { "X", "Z" }
            };

            target.IgnoreTargets = new List<DumpIgnoreTarget> { toIgnore };

            var actual = target.DumpObject(toDump);
            Assert.IsNotNull(actual);
            Assert.AreEqual(toDump.GetType().FullName, actual.Type);

            Assert.AreEqual(0, actual.Fields.Count);
            Assert.AreEqual(1, actual.AutoProperties.Count);
        }

        #endregion

        #region DumpPrimitive

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DumpPrimitive_NullArgument()
        {
            var target = new ObjectDumper();
            target.DumpPrimitive(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DumpPrimitive_NonPrimitive()
        {
            var target = new ObjectDumper();
            target.DumpPrimitive(new TestPoint());
        }

        [TestMethod]
        public void DumpPrimitive_AllPrimitives()
        {
            var target = new ObjectDumper();

            // Integer
            int intValue = (int)5;
            Xml.Primitive actual = target.DumpPrimitive(intValue);
            Assert.AreEqual(typeof(int).FullName, actual.Type);
            Assert.AreEqual(intValue.ToString(), actual.Value);

            // Unsigned int
            uint uintValue = 5;
            actual = target.DumpPrimitive(uintValue);
            Assert.AreEqual(typeof(uint).FullName, actual.Type);
            Assert.AreEqual(uintValue.ToString(), actual.Value);

            // Short
            short shortValue = 5;
            actual = target.DumpPrimitive(shortValue);
            Assert.AreEqual(typeof(short).FullName, actual.Type);
            Assert.AreEqual(shortValue.ToString(), actual.Value);

            // Unsigned short
            ushort ushortValue = 5;
            actual = target.DumpPrimitive(ushortValue);
            Assert.AreEqual(typeof(ushort).FullName, actual.Type);
            Assert.AreEqual(ushortValue.ToString(), actual.Value);

            // Long
            long longValue = 5;
            actual = target.DumpPrimitive(longValue);
            Assert.AreEqual(typeof(long).FullName, actual.Type);
            Assert.AreEqual(longValue.ToString(), actual.Value);

            // Unsigned long
            ulong ulongValue = 5;
            actual = target.DumpPrimitive(ulongValue);
            Assert.AreEqual(typeof(ulong).FullName, actual.Type);
            Assert.AreEqual(ulongValue.ToString(), actual.Value);

            // Double
            double doubleValue = 5.75;
            actual = target.DumpPrimitive(doubleValue);
            Assert.AreEqual(typeof(double).FullName, actual.Type);
            Assert.AreEqual(doubleValue.ToString(), actual.Value);

            // Decimal
            decimal decimalValue = 5.75M;
            actual = target.DumpPrimitive(decimalValue);
            Assert.AreEqual(typeof(decimal).FullName, actual.Type);
            Assert.AreEqual(decimalValue.ToString(), actual.Value);

            // Float
            float floatValue = 5.75F;
            actual = target.DumpPrimitive(floatValue);
            Assert.AreEqual(typeof(float).FullName, actual.Type);
            Assert.AreEqual(floatValue.ToString(), actual.Value);

            // Bool
            bool boolValue = false;
            actual = target.DumpPrimitive(boolValue);
            Assert.AreEqual(typeof(bool).FullName, actual.Type);
            Assert.AreEqual(boolValue.ToString(), actual.Value);

            // Byte
            byte byteValue = 0xff;
            actual = target.DumpPrimitive(byteValue);
            Assert.AreEqual(typeof(byte).FullName, actual.Type);
            Assert.AreEqual(byteValue.ToString(), actual.Value);

            // Signed byte
            sbyte sbyteValue = 0x79;
            actual = target.DumpPrimitive(sbyteValue);
            Assert.AreEqual(typeof(sbyte).FullName, actual.Type);
            Assert.AreEqual(sbyteValue.ToString(), actual.Value);

            // String
            string stringValue = "Hello World";
            actual = target.DumpPrimitive(stringValue);
            Assert.AreEqual(typeof(string).FullName, actual.Type);
            Assert.AreEqual(stringValue.ToString(), actual.Value);
        }

        #endregion

        #endregion

        #region Test Classes

        /// <summary>
        /// A class with only private, readonly fields of all primitive types set to their default .NET CLR values.
        /// </summary>
        private class AllPrimitiveFields
        {
            // Set a variable number of access restrictions
            private readonly int intField;
            public readonly short shortField;
            protected readonly byte byteField;
            private static readonly long longField;
            public static readonly decimal decimalField;
            protected static readonly double doubleField;

            private readonly bool boolField;
            private readonly string stringField;
            private readonly char charField;
            private readonly uint unsignedIntField;
            private readonly ulong unsignedLongField;
            private readonly ushort unsignedShortField;
        }

        /// <summary>
        /// A class with only private auto-properties for all primitive types set to their default .NET CLR values.
        /// </summary>
        private class AllProperties
        {
            // Set a variable number of access restrictions
            private int IntField { get; set; }
            public short ShortField { get; set; }
            protected byte ByteField { get; set; }
            private static long LongField { get; set; }
            public static decimal DecimalField { get; set; }
            protected static double DoubleField { get; set; }

            // Change the accessor restrictions
            public bool BoolField { get; set; }
            public string StringField { private get; set; }
            public char CharField { protected get; set; }
            public uint UnsignedIntField { get; private set; }
            public ulong UnsignedLongField { get; protected set; }
            public ushort UnsignedShortField { get; set; }
        }

        private class FieldsAndProperties
        {
            // Private fields
            private bool boolField;
            private readonly string stringField;
            protected readonly char charField;
            public readonly uint unsignedIntField;
            public readonly ulong unsignedLongField;
            private readonly ushort unsignedShortField;

            // Auto-Properties
            private int IntProp { get; set; }
            public short ShortProp { get; set; }
            protected byte ByteProp { get; set; }
            private static long LongProp { get; set; }
            public static decimal DecimalProp { get; set; }
            protected static double DoubleProp { get; set; }

            // Fields that access properties
            public bool GetSetBoolProp
            {
                get
                {
                    return this.boolField;
                }
                set
                {
                    this.boolField = value;
                }
            }

            public string GetStringField
            {
                get
                {
                    return this.stringField;
                }
            }
        }

        /// <summary>
        /// A test struct with properties and fields.
        /// </summary>
        private struct TestPoint
        {
            private int x;
            private int y;

            public TestPoint(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public int X
            {
                get
                {
                    return this.x;
                }
            }

            public int Y
            {
                get
                {
                    return this.y;
                }
            }
        }

        /// <summary>
        /// A test class that references itself.
        /// </summary>
        private class SelfReference
        {
            private SelfReference reference;

            public SelfReference()
            {
                this.reference = this;
            }
        }

        #endregion

        //#region Object Dumper Impl

        ///// <summary>
        ///// Logs objects of various types, converting them to a simple XML representation.
        ///// </summary>
        //public sealed class ObjectDumper
        //{
        //    #region Fields & Constants

        //    private HashSet<string> ignoreTargetLookup = new HashSet<string>();

        //    #endregion

        //    #region Properties

        //    /// <summary>
        //    /// Sets a list of values that the object dumper will ignore. This setter will push all the
        //    /// values inside the list of ignore targets into an internal HashSet based lookup. 
        //    /// </summary>
        //    public IEnumerable<DumpIgnoreTarget> IgnoreTargets
        //    {
        //        set
        //        {
        //            if (value != null)
        //            {
        //                // This looks quite ugly but it basically concatenates the type name with the field and property
        //                // names stored inside the ignore types.
        //                var toIgnore = value.SelectMany(i => i.FieldAndPropertyNames,
        //                    (t, n) => string.Join(".", t.Type, n));
        //                foreach (var item in toIgnore)
        //                {
        //                    ignoreTargetLookup.Add(item);
        //                }
        //            }
        //        }
        //    }

        //    #endregion

        //    #region Public Methods

        //    /// <summary>
        //    /// In essence, this is a factory method for creating Xml.Object instances. This only works for non-primitive
        //    /// objects and structs.
        //    /// </summary>
        //    /// <param name="target">
        //    /// The target to dump.
        //    /// </param>
        //    /// <param name="maxDepth">
        //    /// The maximum depth to go when recursively logging instance fields and properties. Defaults to 3.
        //    /// </param>
        //    /// <returns>
        //    /// An Xml.Object instance representing the passed in target.
        //    /// </returns>
        //    public Xml.Object DumpObject(object target, ushort maxDepth = 3)
        //    {
        //        Xml.Object obj;
        //        if (target == null)
        //        {
        //            obj = new Xml.Null();
        //        }
        //        else
        //        {
        //            if (maxDepth > 0)
        //            {
        //                if (Objects.IsPrimitive(target))
        //                {
        //                    throw new ArgumentException("Cannot dump a primitive type from this method!");
        //                }
        //                else
        //                {
        //                    var fieldsAndProps = DumpFieldsAndProperties(target, maxDepth);
        //                    obj = new Xml.Object()
        //                    {
        //                        Type = target.GetType().FullName,
        //                        Fields = fieldsAndProps.Fields,
        //                        AutoProperties = fieldsAndProps.Properties
        //                    };
        //                }
        //            }
        //            else
        //            {
        //                // Target will never be null here, that'll get caught earlier.
        //                obj = (Xml.Object)new Xml.NotNull();
        //            }
        //        }

        //        return obj;
        //    }

        //    /// <summary>
        //    /// Creates Xml.Primitive instances. This only works for objects that are considered to be
        //    /// "primtive," which isn't precisly correlated to the obj.GetType().IsPrimitive property. There
        //    /// are a couple of other CLR types that can be considered primitives for our purposes here
        //    /// even if there are no IL instructions that operate on them (which is the CLR's definition of
        //    /// a primitive as of now).
        //    /// </summary>
        //    /// <param name="target">
        //    /// The target primitive object to dump.
        //    /// </param>
        //    /// <returns>
        //    /// An Xml.Primitive object representing the passed in target.
        //    /// </returns>
        //    public Xml.Primitive DumpPrimitive(object target)
        //    {
        //        if (target == null)
        //        {
        //            throw new ArgumentNullException("target");
        //        }

        //        Xml.Primitive primitive;
        //        if (Objects.IsPrimitive(target))
        //        {
        //            primitive = new Xml.Primitive()
        //            {
        //                Type = target.GetType().FullName,
        //                Value = target.ToString()
        //            };
        //        }
        //        else
        //        {
        //            throw new ArgumentException("Cannot dump non-primitive type from this method!");
        //        }

        //        return primitive;
        //    }

        //    #endregion

        //    #region Private Methods

        //    /// <summary>
        //    /// Dump all fields and properties of the target object up to the maximum supplied depth. Note that
        //    /// only auto-generated properties will be dumped, and the fields corresponding to those properties
        //    /// will be pushed into the property as the backing field element.
        //    /// </summary>
        //    /// <param name="target">
        //    /// The target to dump.
        //    /// </param>
        //    /// <param name="maxDepth">
        //    /// The maximum depth to dump.
        //    /// </param>
        //    /// <returns>
        //    /// A FieldsAndProperties class containing the dumped fields and properties.
        //    /// </returns>
        //    private FieldsAndProperties DumpFieldsAndProperties(object target, ushort maxDepth)
        //    {
        //        var dumpedFields = new List<Xml.Field>();
        //        var dumpedProps = new List<Xml.Property>();
        //        var compilerGeneratedFields = new List<FieldInfo>();

        //        // First, spin through the fields.
        //        foreach (var field in target.GetType().GetFields(
        //            BindingFlags.Instance |
        //            BindingFlags.NonPublic |
        //            BindingFlags.Public |
        //            BindingFlags.Static)
        //            .Where(x => !this.IsTargetIgnored(x.DeclaringType.FullName, x.Name)))
        //        {
        //            if (!(Members.IsCompilerGenerated(field) || Objects.CouldBeAnonymousType(target)))
        //            {
        //                dumpedFields.Add(DumpField(target, field, maxDepth));
        //            }
        //            else
        //            {
        //                compilerGeneratedFields.Add(field);
        //            }
        //        }

        //        // Next, spin through the properties, looking for only auto generated
        //        // ones. Match the compiler generated field with the property.
        //        foreach (var prop in target.GetType().GetProperties(
        //            BindingFlags.Instance |
        //            BindingFlags.Static |
        //            BindingFlags.NonPublic |
        //            BindingFlags.Public))
        //        {
        //            if (prop.GetIndexParameters().Length == 0 &&
        //                (Objects.CouldBeAnonymousType(target) || Members.MightBeAutoGenerated(prop)))
        //            {
        //                // Look for backing field.
        //                var backingField = FindBackingField(target, compilerGeneratedFields, prop);
        //                if (backingField != null)
        //                {
        //                    if (!this.IsTargetIgnored(prop.DeclaringType.FullName, prop.Name))
        //                    {
        //                        dumpedProps.Add(DumpProperty(target, backingField, prop, maxDepth));
        //                    }

        //                    // remove backing field no matter what, else field counts will be wrong
        //                    compilerGeneratedFields.Remove(backingField);
        //                }
        //            }
        //        }

        //        // Process what's left of the backing fields list--just in case something didn't line up with a property.
        //        foreach (var leftoverField in compilerGeneratedFields)
        //        {
        //            dumpedFields.Add(DumpField(target, leftoverField, maxDepth));
        //        }

        //        return new FieldsAndProperties(dumpedFields, dumpedProps);
        //    }

        //    /// <summary>
        //    /// Finds the backing field for the target property.
        //    /// </summary>
        //    /// <param name="target">
        //    /// The target object.
        //    /// </param>
        //    /// <param name="compilerGeneratedFields">
        //    /// The list of fields to search.
        //    /// </param>
        //    /// <param name="prop">
        //    /// The property whose backing field to find.
        //    /// </param>
        //    /// <returns>
        //    /// The backing field.
        //    /// </returns>
        //    private static FieldInfo FindBackingField(object target, List<FieldInfo> compilerGeneratedFields, PropertyInfo prop)
        //    {
        //        return compilerGeneratedFields.FirstOrDefault(
        //            (x) =>
        //            {
        //                return x.Name.StartsWith("<" + prop.Name + ">") &&
        //                    ((Objects.CouldBeAnonymousType(target) && x.Name.Contains(Members.AnonymousTypeFieldDesignation)) ||
        //                    x.Name.Contains(Members.BackingFieldDesignation));
        //            });
        //    }

        //    /// <summary>
        //    /// Dumps the target field and any containing members up to the supplied depth.
        //    /// </summary>
        //    /// <param name="target">
        //    /// The target object.
        //    /// </param>
        //    /// <param name="field">
        //    /// The field to dump.
        //    /// </param>
        //    /// <param name="maxDepth">
        //    /// The maximum depth to traverse members.
        //    /// </param>
        //    /// <returns>
        //    /// The dumped field.
        //    /// </returns>
        //    private Xml.Field DumpField(object target, FieldInfo field, ushort maxDepth)
        //    {
        //        var dumpedField = new Xml.Field()
        //        {
        //            Name = field.Name,
        //            Value = new Xml.Value()
        //        };

        //        var value = field.GetValue(target);
        //        if (value == null)
        //        {
        //            dumpedField.Value.Item = new Xml.Null();
        //        }
        //        else if (value == target)
        //        {
        //            dumpedField.Value.Item = new Xml.This();
        //        }
        //        else if (Objects.IsPrimitive(value))
        //        {
        //            dumpedField.Value.Item = DumpPrimitive(value);
        //        }
        //        else
        //        {
        //            dumpedField.Value.Item = DumpObject(value, (ushort)(maxDepth - 1));
        //        }

        //        return dumpedField;
        //    }

        //    /// <summary>
        //    /// Dumps the target field and any containing members up to the supplied depth.
        //    /// </summary>
        //    /// <param name="target">
        //    /// The target object.
        //    /// </param>
        //    /// <param name="backingField">
        //    /// The backing field of the target property.
        //    /// </param>
        //    /// <param name="prop">
        //    /// The property to dump.
        //    /// </param>
        //    /// <param name="maxDepth">
        //    /// The maximum depth to traverse members.
        //    /// </param>
        //    /// <returns>
        //    /// The dumped property.
        //    /// </returns>
        //    private Xml.Property DumpProperty(object target, FieldInfo backingField, PropertyInfo prop, ushort maxDepth)
        //    {
        //        var dumpedProperty = new Xml.Property()
        //        {
        //            Name = prop.Name,
        //            Value = new Xml.Value(),
        //            BackingField = DumpField(target, backingField, (ushort)(maxDepth - 1))
        //        };

        //        var value = prop.GetValue(target, null);
        //        if (value == null)
        //        {
        //            dumpedProperty.Value.Item = new Xml.Null();
        //        }
        //        else if (value == target)
        //        {
        //            dumpedProperty.Value.Item = new Xml.This();
        //        }
        //        else if (Objects.IsPrimitive(value))
        //        {
        //            dumpedProperty.Value.Item = DumpPrimitive(value);
        //        }
        //        else
        //        {
        //            dumpedProperty.Value.Item = DumpObject(value, (ushort)(maxDepth - 1));
        //        }

        //        return dumpedProperty;
        //    }

        //    /// <summary>
        //    /// Checks to see if the target type and field or property name exists in the
        //    /// ignore targets list.
        //    /// </summary>
        //    /// <param name="type">
        //    /// The type name.
        //    /// </param>
        //    /// <param name="name">
        //    /// the field or property name.
        //    /// </param>
        //    /// <returns>
        //    /// True if the ignore type is in the lookup, false otherwise.
        //    /// </returns>
        //    private bool IsTargetIgnored(string type, string name)
        //    {
        //        var qualifiedMember = string.Join(".", type, name);
        //        return this.ignoreTargetLookup.Contains(qualifiedMember);
        //    }

        //    #endregion

        //    #region Private Classes

        //    /// <summary>
        //    /// Simple inner class to represent a combined return value of the field/property dumping
        //    /// algorithm. The need for this stemmed from a combining of the logic to generate both lists
        //    /// for performance reasons.
        //    /// </summary>
        //    private class FieldsAndProperties
        //    {
        //        #region Constructors & Destructors

        //        public FieldsAndProperties(List<Xml.Field> fields, List<Xml.Property> properties)
        //        {
        //            if (fields == null || properties == null)
        //            {
        //                throw new ArgumentNullException(fields == null ? "fields" : "properties");
        //            }

        //            this.Fields = fields;
        //            this.Properties = properties;
        //        }

        //        #endregion

        //        #region Properties

        //        /// <summary>
        //        /// Gets a list of fields.
        //        /// </summary>
        //        public List<Xml.Field> Fields { get; private set; }

        //        /// <summary>
        //        /// Gets the list of properties.
        //        /// </summary>
        //        public List<Xml.Property> Properties { get; private set; }

        //        #endregion
        //    }

        //    #endregion

        //}

        ///// <summary>
        ///// Contains extension methods for classes dealing with reflection based class members.
        ///// </summary>
        //public static class Members
        //{
        //    #region Fields & Constants

        //    public static readonly string BackingFieldDesignation = "_BackingField";
        //    public static readonly string AnonymousTypeFieldDesignation = "_Field";

        //    #endregion

        //    #region Extension Methods

        //    /// <summary>
        //    /// Detects if the target member is compiler generated.
        //    /// </summary>
        //    /// <param name="info">
        //    /// This MemberInfo.
        //    /// </param>
        //    /// <returns>
        //    /// True if the compiler generated attribute is set, false otherwise.
        //    /// </returns>
        //    public static bool IsCompilerGenerated(MemberInfo info)
        //    {
        //        return info.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any();
        //    }

        //    /// <summary>
        //    /// Detects if the target property is automatically implemented.
        //    /// </summary>
        //    /// <param name="prop">
        //    /// The property to test. 
        //    /// </param>
        //    /// <returns>
        //    /// True if the property is auto implemented, false otherwise.
        //    /// </returns>
        //    public static bool MightBeAutoGenerated(PropertyInfo prop)
        //    {
        //        var getMethod = prop.GetGetMethod();
        //        var setMethod = prop.GetSetMethod();
        //        return (getMethod == null || IsCompilerGenerated(getMethod)) && (setMethod == null || IsCompilerGenerated(setMethod));
        //    }

        //    #endregion
        //}

        ///// <summary>
        ///// Object extension methods.
        ///// </summary>
        //public static class Objects
        //{
        //    #region Fields & Constants

        //    public static readonly string AnonymousTypeDesignation = "AnonymousType";

        //    #endregion

        //    #region Extension Methods

        //    /// <summary>
        //    /// Determines if the target object is primitive.
        //    /// </summary>
        //    /// <param name="obj">
        //    /// The object to test.
        //    /// </param>
        //    /// <returns>
        //    /// True if the object is primitive, false otherwise.
        //    /// </returns>
        //    public static bool IsPrimitive(object obj)
        //    {
        //        return obj.GetType().IsPrimitive || obj is decimal || obj is string;
        //    }

        //    /// <summary>
        //    /// Determins if the target object *might* be an anonymous type. You could break this by simply
        //    /// creating a class with the CompilerGenerated attribute and adding "AnonymousType" to the name.
        //    /// </summary>
        //    /// <param name="obj">
        //    /// The object to test.
        //    /// </param>
        //    /// <returns></returns>
        //    public static bool CouldBeAnonymousType(object obj)
        //    {
        //        var type = obj.GetType();
        //        return Members.IsCompilerGenerated(type) && type.FullName.Contains(AnonymousTypeDesignation) &&
        //            type.Name.StartsWith("<>") && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        //    }

        //    #endregion
        //}

        //#endregion
    }
}
