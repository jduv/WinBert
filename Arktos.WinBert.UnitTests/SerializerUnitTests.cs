namespace Arktos.WinBert.UnitTests
{
    using System;
    using Arktos.WinBert.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;

    [TestClass]
    public class SerializerUnitTests
    {
        #region Test Methods

        #region XmlSerialize
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Serialize_NullArgument()
        {
            var target = Serializer.XmlSerialize(null);
        }

        #endregion

        #region XmlDeserialize

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void XmlDeserialize_NullStringArgument()
        {
            var target = Serializer.XmlDeserialize<string>((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void XmlDeserialize_NullStreamArgument()
        {
            var target = Serializer.XmlDeserialize<string>((Stream)null);
        }

        #endregion

        #endregion
    }
}
