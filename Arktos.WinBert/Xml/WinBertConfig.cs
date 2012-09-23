namespace Arktos.WinBert.Xml
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>    
    public partial class WinBertConfig
    {
        #region Public Methods

        /// <summary>
        /// Deserilizes the target stream into a configuration object.
        /// </summary>
        /// <param name="stream">The stream to deserialize.</param>
        /// <returns>A deserialized configuration object.</returns>
        public static WinBertConfig Deserialize(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(WinBertConfig));
            using (stream)
            {
                return (WinBertConfig) serializer.Deserialize(stream);
            }            
        }

        #endregion
    }
}
