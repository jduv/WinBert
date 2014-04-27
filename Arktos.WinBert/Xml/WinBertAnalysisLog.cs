namespace Arktos.WinBert.Xml
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed partial class WinBertAnalysisLog
    {
    }

    /// <summary>
    /// Partial implementation of a partial class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed partial class TestExecution
    {
    }

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed partial class Value
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether the wrapped item is null. Utility property.
        /// </summary>
        public bool IsNull
        {
            get
            {
                return this.Item == null;
            }
        }

        /// <summary>
        /// Gets the wrapped item as an Xml.Object instance. This could return null. Utility property.
        /// </summary>
        public Xml.Object AsObject
        {
            get
            {
                return this.Item as Xml.Object;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the wrapped item is an Xml.Object instance. Utility property.
        /// </summary>
        public bool IsObject
        {
            get
            {
                return this.AsObject != null;
            }
        }

        /// <summary>
        /// Gets the wrapped item as an Xml.Primitive instance. This could return null. Utility property.
        /// </summary>
        public Xml.Primitive AsPrimitive
        {
            get
            {
                return this.Item as Xml.Primitive;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the wrapped item is an Xml.Primitive instance. Utility property.
        /// </summary>
        public bool IsPrimtive
        {
            get
            {
                return this.AsPrimitive != null;
            }
        }

        /// <summary>
        /// Gets the underlying type of the wrapped item. If the wrapped item is null, this will return null.
        /// </summary>
        public Type UnderlyingType
        {
            get
            {
                return this.IsNull ? null : this.Item.GetType();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks to determing if this value is comparable to the target one. 
        /// </summary>
        /// <param name="that">
        /// The target Xml.Value to compare against.
        /// </param>
        /// <returns>
        /// True if both values are comparable to one another, false otherwise.
        /// </returns>
        public bool IsComparableTo(Value that)
        {
            bool comparable = true;
            if (that == null || this.Item == null || that.Item == null)
            {
                comparable = false;
            }
            else
            {
                comparable = object.ReferenceEquals(this, that) || this.Item.GetType() == that.Item.GetType();
            }

            return comparable;
        }

        #endregion
    }

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed partial class MethodCall
    {
    }

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public partial class Object
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether the current instance of this class is a Null sential type.
        /// </summary>
        public bool IsNullType
        {
            get
            {
                return this.GetType() == typeof(Null);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current instance of this class is a NotNull sentinal type.
        /// </summary>
        public bool IsNotNullType
        {
            get
            {
                return this.GetType() == typeof(NotNull);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current instance of this class is a This sentinal type.
        /// </summary>
        public bool IsThisType
        {
            get
            {
                return this.GetType() == typeof(This);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current instance of this class is a sentinal type.
        /// </summary>
        public bool IsSentinalType
        {
            get
            {
                return this.IsNotNullType || this.IsNullType | this.IsThisType;
            }
        }

        #endregion
    }

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed partial class Primitive
    {
        #region Public Methods

        /// <summary>
        /// Returns a value indicating whether this primitive and the target argument can be
        /// compared. This should only return true if the underlying data types are equal.
        /// </summary>
        /// <param name="that">
        /// The object to compare with.
        /// </param>
        /// <returns>
        /// True if both underlying data types are equal by full name comparison. Case sensitive.
        /// </returns>
        public bool IsComparableTo(Primitive that)
        {
            return this.FullName != null && this.FullName.Equals(that.FullName);
        }

        /// <inheritdoc />
        public bool Equals(Primitive that)
        {
            return object.ReferenceEquals(this, that) ||
                (that != null && this.IsComparableTo(that) && that.Value.Equals(this.Value));
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Primitive ? this.Equals(obj as Primitive) : false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.FullName.GetHashCode() * 37 + this.Value.GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed partial class Field
    {
    }

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed partial class Property
    {
    }

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed partial class Null
    {
        #region Fields & Constants

        private static readonly string NullValue = "null";

        #endregion

        #region Properties

        /// <summary>
        /// Gets a string value representing null.
        /// </summary>
        public string Value
        {
            get
            {
                return NullValue;
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        /// <remarks>
        /// A Null value is equal to any other Null value, so we only check for type
        /// equality.
        /// </remarks>
        public override bool Equals(object obj)
        {
            return obj is Null;
        }

        /// <inheritdoc />
        /// <remarks>
        /// This may look a little funny, but all Null types are equal, and thereby should hash
        /// equally where possible.
        /// </remarks>
        public override int GetHashCode()
        {
            return this.GetType().GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed partial class NotNull
    {
        #region Fields & Constants

        private static readonly string NotNullValue = "not null";

        #endregion

        #region Properties

        /// <summary>
        /// Gets a string value representing null.
        /// </summary>
        public string Value
        {
            get
            {
                return NotNullValue;
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        /// <remarks>
        /// A NotNull value is equal to any other NotNull value, so we only check for type
        /// equality.
        /// </remarks>
        public override bool Equals(object obj)
        {
            return obj is NotNull;
        }

        /// <inheritdoc />
        /// <remarks>
        /// This may look a little funny, but all NotNull types are equal, and thereby should hash
        /// equally where possible.
        /// </remarks>
        public override int GetHashCode()
        {
            return this.GetType().GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// Partial implementation of a generated class.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed partial class This
    {
        #region Fields & Constants

        private static readonly string ThisValue = "this";

        #endregion

        #region Properties

        /// <summary>
        /// Gets a string value representing null.
        /// </summary>
        public string Value
        {
            get
            {
                return ThisValue;
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        /// <remarks>
        /// A This value is equal to any other This value, so we only check for type
        /// equality.
        /// </remarks>
        public override bool Equals(object obj)
        {
            return obj is This;
        }

        /// <inheritdoc />
        /// <remarks>
        /// This may look a little funny, but all This types are equal, and thereby should hash
        /// equally where possible.
        /// </remarks>
        public override int GetHashCode()
        {
            return this.GetType().GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// Partial implementation of a partial class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed partial class DynamicCallGraph
    {
    }

    /// <summary>
    /// Partial implementation of a partial class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed partial class CallGraphNode
    {
    }
}
