namespace Arktos.WinBert.UnitTests
{
    using Arktos.WinBert.Instrumentation;
    using Arktos.WinBert.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class ObjectDumperUnitTest
    {
        #region Fields & Constants

        private static readonly string NullDump = @"<Null xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" />";

        private static readonly string NotNullDump = @"<NotNull xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" />";

        #endregion

        #region Test Methods

        #region DumpObject

        [TestMethod]
        public void DumpObject_NullArgument()
        {
            var target = new ObjectDumper();
            var actual = target.DumpObject(null);
            Assert.AreEqual(NullDump, Serializer.XmlSerialize(actual, asFragment: true));
        }

        [TestMethod]
        public void DumpObject_NullArgument_ZeroDepth()
        {
            var target = new ObjectDumper();
            var actual = target.DumpObject(null, maxDepth: 0);
            Assert.AreEqual(NullDump, Serializer.XmlSerialize(actual, asFragment: true));
        }

        [TestMethod]
        public void DumpObject_NonNullArgument_ZeroDepth()
        {
            var target = new ObjectDumper();
            var actual = target.DumpObject(new { x = 1, y = 2 }, maxDepth: 0);
            Assert.AreEqual(NotNullDump, Serializer.XmlSerialize(actual, asFragment: true));
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
            // BMK Reproduces issue #4
            var now = DateTime.Now;
            var target = new ObjectDumper();

            // For now, we set the depth to 2 to keep from blowing up the stack.
            var actual = target.DumpObject(now, 2);
            Assert.AreEqual(now.GetType().FullName, actual.Type);
        }

        [TestMethod]
        public void DumpObject_ValueType()
        {
            var toDump = new TestPoint();
            var target = new ObjectDumper();
            var actual = target.DumpObject(toDump);

            Assert.AreEqual(toDump.GetType().FullName, actual.Type);
            Assert.AreEqual(2, actual.Fields.Length);
            Assert.AreEqual(2, actual.Properties.Length);

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

            // Properties
            var xPropValue = actual.Properties[0].Value.Item as Xml.Primitive;
            var yPropValue = actual.Properties[0].Value.Item as Xml.Primitive;
            Assert.IsNotNull(xPropValue);
            Assert.IsNotNull(yPropValue);

            Assert.AreEqual("X", actual.Properties[0].Name);
            Assert.AreEqual(toDump.X.GetType().FullName, xPropValue.Type);
            Assert.AreEqual(toDump.X.ToString(), xPropValue.Value);

            Assert.AreEqual("Y", actual.Properties[1].Name);
            Assert.AreEqual(toDump.Y.GetType().FullName, yPropValue.Type);
            Assert.AreEqual(toDump.Y.ToString(), yPropValue.Value);
        }

        [TestMethod]
        public void DumpObject_SimpleObjectWithFields()
        {
            var toDump = new AllPrimitiveFields();
            var target = new ObjectDumper();
            var actual = target.DumpObject(toDump);
            Assert.AreEqual(toDump.GetType().FullName, actual.Type);
            Assert.IsNotNull(actual.Fields);
            Assert.IsNotNull(actual.Properties);
            Assert.AreEqual(12, actual.Fields.Length);
            Assert.AreEqual(0, actual.Properties.Length);
        }

        [TestMethod]
        public void DumpObject_SimpleObjectWithProperties()
        {
            var toDump = new AllProperties();
            var target = new ObjectDumper();
            var actual = target.DumpObject(toDump);

            Assert.AreEqual(toDump.GetType().FullName, actual.Type);
            Assert.IsNotNull(actual.Fields);
            Assert.IsNotNull(actual.Properties);
            Assert.AreEqual(12, actual.Fields.Length);
            Assert.AreEqual(12, actual.Properties.Length);
        }

        [TestMethod]
        public void DumpObject_ComplexObjectDepthOfOne()
        {
            var toDump = new { x = 1, y = new { a = 2, b = 3, c = 4 }, z = (string)null };
            var target = new ObjectDumper();
            var actual = target.DumpObject(toDump, 1);
            Assert.AreEqual(toDump.GetType().FullName, actual.Type);
            Assert.IsNotNull(actual.Fields);
            Assert.IsNotNull(actual.Properties);

            Assert.AreEqual(3, actual.Fields.Length);
            Assert.AreEqual(3, actual.Properties.Length);

            Assert.IsNotNull(actual.Fields[0]);
            Assert.IsNotNull(actual.Fields[1]);
            Assert.IsNotNull(actual.Fields[2]);

            Assert.IsNotNull(actual.Properties[0]);
            Assert.IsNotNull(actual.Properties[1]);
            Assert.IsNotNull(actual.Properties[2]);

            // Check first field/prop
            var xFieldValue = actual.Fields[0].Value.Item as Xml.Primitive;
            var xPropValue = actual.Properties[0].Value.Item as Xml.Primitive;
            Assert.IsNotNull(xFieldValue);
            Assert.AreEqual(toDump.x.ToString(), xFieldValue.Value);
            Assert.IsNotNull(xPropValue);
            Assert.AreEqual(toDump.x.ToString(), xPropValue.Value);

            // Check second field/prop
            var yFieldValue = actual.Fields[1].Value.Item as Xml.NotNull;
            var yPropValue = actual.Properties[1].Value.Item as Xml.NotNull;
            Assert.IsNotNull(yFieldValue);
            Assert.IsNotNull(yPropValue);

            // Check last field/prop
            var zFieldValue = actual.Fields[2].Value.Item as Xml.Null;
            var zPropValue = actual.Properties[2].Value.Item as Xml.Null;
            Assert.IsNotNull(zFieldValue);
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

            Assert.AreEqual(1, actual.Fields.Length);
            Assert.AreEqual(0, actual.Properties.Length);

            var refValue = actual.Fields[0].Value.Item as Xml.This;
            Assert.AreEqual("reference", actual.Fields[0].Name);
            Assert.IsNotNull(refValue);
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
    }
}
