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

        private static readonly string IntegerPrimitiveDump = @"";

        private static readonly string PrivatePrimitiveFieldsDump = @"";

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
            var actual = target.DumpObject("Hello World", maxDepth: 0);
            Assert.AreEqual(NotNullDump, Serializer.XmlSerialize(actual, asFragment: true));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DumpObject_PrimitiveTypes()
        {
            var target = new ObjectDumper();
            var actual = target.DumpObject(5);
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
