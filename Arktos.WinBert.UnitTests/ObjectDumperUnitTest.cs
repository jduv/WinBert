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
        public void DumpObject_ValueType()
        {
            var now = DateTime.Now;
            var target = new ObjectDumper();
            var actual = target.DumpObject(now);
            Assert.AreEqual(now.GetType().FullName, actual.Type);
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
        private class AllPrivatePrimitiveFields
        {
            private readonly int intField;
            private readonly short shortField;
            private readonly byte byteField;
            private readonly long longField;
            private readonly decimal decimalField;
            private readonly double doubleField;
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
        private class AllPrivatePrimitiveProperties
        {
            private int IntField { get; set; }
            private short ShortField { get; set; }
            private byte ByteField { get; set; }
            private long LongField { get; set; }
            private decimal DecimalField { get; set; }
            private double DoubleField { get; set; }
            private bool BoolField { get; set; }
            private string StringField { get; set; }
            private char CharField { get; set; }
            private uint UnsignedIntField { get; set; }
            private ulong UnsignedLongField { get; set; }
            private ushort UnsignedShortField { get; set; }
        }

        #endregion
    }
}
