namespace Arktos.WinBert.Xml
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml.Serialization;

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>    
    [ExcludeFromCodeCoverage]
    public partial class WinBertConfig : ISerializable
    {
        #region Fields & Constants

        public static readonly string XmlSerializationKey = "WinBertConfig";

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the WinBertConfig class.
        /// </summary>
        public WinBertConfig()
        {
        }

        /// <summary>
        /// Initializes a new instance of the WinBertConfig class. This is the deserialization constructor.
        /// </summary>
        /// <param name="info">
        /// The serialization info.
        /// </param>
        /// <param name="context">
        /// The streaming context.
        /// </param>
        public WinBertConfig(SerializationInfo info, StreamingContext context)
        {
            var payload = (string)info.GetValue(XmlSerializationKey, typeof(string));

            using (var reader = new StringReader(payload))
            {
                var serializer = new XmlSerializer(typeof(WinBertConfig));
                WinBertConfig config = (WinBertConfig)serializer.Deserialize(reader);

                this.EmbeddedConfigurations = config.EmbeddedConfigurations;
                this.IgnoreList = config.ignoreListField;
                this.MasterArchivePath = config.MasterArchivePath;
                this.Projects = config.Projects;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Deserilizes the target stream into a configuration object.
        /// </summary>
        /// <param name="stream">The stream to deserialize.</param>
        /// <returns>A deserialized configuration object.</returns>
        public static WinBertConfig XmlDeserialize(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(WinBertConfig));
            using (stream)
            {
                return (WinBertConfig)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// Serializes the target WinBertConfig object.
        /// </summary>
        /// <param name="toSerialize">
        /// The object to serialize.
        /// </param>
        /// <returns>
        /// A string representation of the target object.
        /// </returns>
        public static string XmlSerialize(WinBertConfig toSerialize)
        {
            var buffer = new StringBuilder();
            var serializer = new XmlSerializer(typeof(WinBertConfig));
            serializer.Serialize(new StringWriter(buffer), toSerialize);
            return buffer.ToString();
        }

        /// <inheritdoc />
        /// <remarks>
        /// Because a WinBertConfig object has an ANY xml element in it's schema, it's not deserializable
        /// via the standard .NET framework serialization mechanisms. So, in order to pass the configuration around
        /// to remote AppDomain instances, we need to describe how serialization occurs.
        /// </remarks>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Xml serialize the whole thin and ship it as a string.
            info.AddValue(XmlSerializationKey, WinBertConfig.XmlSerialize(this), typeof(string));
        }

        #endregion
    }
}
