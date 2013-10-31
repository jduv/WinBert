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
        /// XML deserializes the target string into an object of type T.
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

            return XmlDeserialize<T>(new MemoryStream(ASCIIEncoding.Default.GetBytes(xml)));
        }

        /// <summary>
        /// Xml deserializes the target stream into an object of type T.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object.
        /// </typeparam>
        /// <param name="stream">
        /// The stream to deserialize.
        /// </param>
        /// <returns>
        /// An object of type T.
        /// </returns>
        public static T XmlDeserialize<T>(Stream stream)
        {
            if (stream == null || !stream.CanRead || stream.Length <= 0)
            {
                throw new ArgumentException("Cannot deserialize a null, empty, or unreadable stream.");
            }

            var serializer = new XmlSerializer(typeof(T));
            using (stream)
            {
                return (T)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// Serializes the target object into XML with default settings.
        /// </summary>
        /// <param name="toSerialize">
        /// The object to serialize.
        /// </param>
        /// <returns>
        /// An XML representation of that object.
        /// </returns>
        public static string XmlSerialize(object toSerialize)
        {
            return XmlSerialize(toSerialize, new XmlWriterSettings());
        }

        /// <summary>
        /// Serializes the target object into XML with the target settings.
        /// </summary>
        /// <param name="toSerialize">
        /// The object to serialize.
        /// </param>
        /// <param name="settings">
        /// The settings to use when writing the XML.
        /// </param>
        /// <returns>
        /// An XML representation of that object.
        /// </returns>
        public static string XmlSerialize(object toSerialize, XmlWriterSettings settings)
        {
            if (toSerialize == null)
            {
                throw new ArgumentNullException("toSerialize");
            }

            var buffer = new StringBuilder();
            using (var writer = XmlWriter.Create(buffer, settings))
            {
                var serializer = new XmlSerializer(toSerialize.GetType());
                serializer.Serialize(writer, toSerialize);
                return buffer.ToString();
            }
        }

        /// <summary>
        /// Serializes the target object into XML with default settings and saving the result
        /// to the target path.
        /// </summary>
        /// <param name="toSerialize">
        /// The object to serialize.
        /// </param>
        /// <param name="path">
        /// The path to save the XML to.
        /// </param>
        public static void XmlSerialize(object toSerialize, string path)
        {
            XmlSerialize(toSerialize, path, new XmlWriterSettings());
        }

        /// <summary>
        /// Serializes the target object into XML with the target settings and saving the result
        /// to the target path.
        /// </summary>
        /// <param name="toSerialize">
        /// The object to serialize.
        /// </param>
        /// <param name="path">
        /// The path to save the XML to.
        /// </param>
        /// <param name="settings">
        /// The settings to use when writing the XML.
        /// </param>
        public static void XmlSerialize(object toSerialize, string path, XmlWriterSettings settings)
        {
            if (toSerialize == null)
            {
                throw new ArgumentNullException("toSerialize");
            }

            using (var writer = XmlWriter.Create(path, settings))
            {
                var serializer = new XmlSerializer(toSerialize.GetType());
                serializer.Serialize(writer, toSerialize);
            }
        }

        #endregion
    }
}
