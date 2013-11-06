namespace Arktos.WinBert.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>    
    [ExcludeFromCodeCoverage]
    public sealed partial class WinBertConfig : ISerializable
    {
        #region Fields & Constants

        public static readonly string XmlSerializationKey = "WinBertConfig";

        #endregion

        #region Constructors & Destructors

        public WinBertConfig()
        {
        }

        public WinBertConfig(SerializationInfo info, StreamingContext context)
        {
            var payload = (string)info.GetValue(XmlSerializationKey, typeof(string));

            using (var reader = new StringReader(payload))
            {
                var serializer = new XmlSerializer(typeof(WinBertConfig));
                WinBertConfig config = (WinBertConfig)serializer.Deserialize(reader);

                this.EmbeddedConfigurations = config.EmbeddedConfigurations;
                this.DiffIgnoreList = config.diffIgnoreListField;
                this.MasterArchivePath = config.MasterArchivePath;
                this.Projects = config.Projects;
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        /// <remarks>
        /// Because a WinBertConfig object has an ANY xml element in it's schema, it's not deserializable
        /// via the standard .NET framework serialization mechanisms. So, in order to pass the configuration around
        /// to remote AppDomain instances, we need to describe how serialization occurs.
        /// </remarks>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Xml serialize the whole thin and ship it as a string.
            info.AddValue(XmlSerializationKey, Serializer.XmlSerialize(this), typeof(string));
        }

        #endregion
    }

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed partial class Build : IEquatable<Build>
    {
        #region Constructors and Destructors

        public Build()
        {
        }

        public Build(uint sequenceNumber, string AssemblyPath)
        {
            this.SequenceNumber = sequenceNumber;
            this.AssemblyPath = AssemblyPath;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        /// <remarks>
        /// Override of Equals to make comparisons easier.
        /// </remarks>
        /// <param name="obj">
        /// The object to compare ourselves to.
        /// </param>
        /// <returns>
        /// True if the objects are equal, false otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Build)
            {
                return this.Equals(obj as Build);
            }

            return false;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Compares this build to the target build object.
        /// </remarks>
        /// <param name="build">
        /// The build to compare to.
        /// </param>
        /// <returns>
        /// True if the builds are equal, false otherwise.
        /// </returns>
        public bool Equals(Build build)
        {
            return this.SequenceNumber == build.SequenceNumber && this.AssemblyPath.Equals(build.AssemblyPath);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Override of GetHashCode for uniqueness.
        /// </remarks>
        /// <returns>
        /// A semi-unique hash code for this object.
        /// </returns>
        public override int GetHashCode()
        {
            return this.SequenceNumber.GetHashCode() ^ this.AssemblyPath.GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public partial class DiffIgnoreTarget
    {
        #region Constructors & Destructors

        public DiffIgnoreTarget()
        {
        }

        public DiffIgnoreTarget(DiffIgnoreType type, string name)
        {
            this.Type = type;
            this.Name = name;
        }

        #endregion
    }

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public partial class DumpIgnoreTarget
    {
        #region Constructors & Destructors

        public DumpIgnoreTarget()
        {
        }

        public DumpIgnoreTarget(string type, string[] fieldAndPropertyNames)
        {
            this.Type = type;
            this.FieldAndPropertyNames = fieldAndPropertyNames;
        }

        #endregion
    }

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed partial class EmbeddedConfiguration
    {
    }

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public partial class WinBertProject
    {
    }
}
