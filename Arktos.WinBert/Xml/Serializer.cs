namespace Arktos.WinBert.Xml
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using System.Xml;

    /// <summary>
    /// Utility class for serialization/deserialization.
    /// </summary>
    public static class Serializer
    {
        #region Public Methods

        /// <summary>
        /// XML de-serializes the target string into an object of type T.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object.
        /// </typeparam>
        /// <param name="xml">
        /// The XML to deserialize.
        /// </param>
        /// <returns>
        /// An object of type T.
        /// </returns>
        public static T XmlDeserialize<T>(string xml)
        {
            if(string.IsNullOrEmpty(xml))
            {
                throw new ArgumentException("Cannot deserialize null or empty XML");
            }

            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(xml)))
            {
                return (T)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// XML serializes the target object.
        /// </summary>
        /// <param name="toSerialize">
        /// The object to serialize.
        /// </param>
        /// <returns>
        /// An XML representation of the target object.
        /// </returns>
        public static string XmlSerialize(object toSerialize, bool asFragment = false)
        {
            if (toSerialize == null)
            {
                throw new ArgumentNullException("toSerialize");
            }

            var buffer = new StringBuilder();

            using (var writer = XmlWriter.Create(buffer, new XmlWriterSettings() { OmitXmlDeclaration = asFragment }))
            {
                var serializer = new XmlSerializer(toSerialize.GetType());
                serializer.Serialize(writer, toSerialize);
                return buffer.ToString();
            }
        }

        #endregion
    }
}
