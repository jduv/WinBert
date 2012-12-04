namespace Arktos.WinBert.UnitTests
{
    using System;
    using Arktos.WinBert.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void XmlDeserialize_NullArgument()
        {
            var target = Serializer.XmlDeserialize<string>(null);
        }

        #endregion

        #endregion
    }
}
