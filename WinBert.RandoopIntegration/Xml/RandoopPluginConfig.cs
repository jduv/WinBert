﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.5444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 
namespace WinBert.RandoopIntegration.Xml {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://arktos.org/RandoopPluginConfig.xsd", IsNullable=false)]
    public partial class RandoopPluginConfig {
        
        private ForbidExpression[] forbiddenTypesField;
        
        private ForbidExpression[] forbiddenFieldsField;
        
        private ForbidExpression[] forbiddenMembersField;
        
        private RandoopPluginConfigSeedValues seedValuesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ForbiddenType", IsNullable=false)]
        public ForbidExpression[] ForbiddenTypes {
            get {
                return this.forbiddenTypesField;
            }
            set {
                this.forbiddenTypesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ForbiddenField", IsNullable=false)]
        public ForbidExpression[] ForbiddenFields {
            get {
                return this.forbiddenFieldsField;
            }
            set {
                this.forbiddenFieldsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ForbiddenField", IsNullable=false)]
        public ForbidExpression[] ForbiddenMembers {
            get {
                return this.forbiddenMembersField;
            }
            set {
                this.forbiddenMembersField = value;
            }
        }
        
        /// <remarks/>
        public RandoopPluginConfigSeedValues SeedValues {
            get {
                return this.seedValuesField;
            }
            set {
                this.seedValuesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    public partial class ForbidExpression {
        
        private string patternField;
        
        private PatternType typeField;
        
        /// <remarks/>
        public string Pattern {
            get {
                return this.patternField;
            }
            set {
                this.patternField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public PatternType Type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    public enum PatternType {
        
        /// <remarks/>
        Regex,
        
        /// <remarks/>
        ExactString,
        
        /// <remarks/>
        Wildcard,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    public partial class RandoopPluginConfigSeedValues {
        
        private RandoopPluginConfigSeedValuesByteSeedValues byteSeedValuesField;
        
        private RandoopPluginConfigSeedValuesUByteSeedValues uByteSeedValuesField;
        
        private RandoopPluginConfigSeedValuesShortSeedValues shortSeedValuesField;
        
        private RandoopPluginConfigSeedValuesUShortSeedValues uShortSeedValuesField;
        
        private RandoopPluginConfigSeedValuesIntSeedValues intSeedValuesField;
        
        private RandoopPluginConfigSeedValuesUIntSeedValues uIntSeedValuesField;
        
        private RandoopPluginConfigSeedValuesLongSeedValues longSeedValuesField;
        
        private RandoopPluginConfigSeedValuesULongSeedValues uLongSeedValuesField;
        
        private RandoopPluginConfigSeedValuesDoubleSeedValues doubleSeedValuesField;
        
        private RandoopPluginConfigSeedValuesFloatSeedValues floatSeedValuesField;
        
        private RandoopPluginConfigSeedValuesDecimalSeedValues decimalSeedValuesField;
        
        private RandoopPluginConfigSeedValuesBoolSeedValues boolSeedValuesField;
        
        private RandoopPluginConfigSeedValuesStringSeedValues stringSeedValuesField;
        
        private RandoopPluginConfigSeedValuesCharSeedValues charSeedValuesField;
        
        /// <remarks/>
        public RandoopPluginConfigSeedValuesByteSeedValues ByteSeedValues {
            get {
                return this.byteSeedValuesField;
            }
            set {
                this.byteSeedValuesField = value;
            }
        }
        
        /// <remarks/>
        public RandoopPluginConfigSeedValuesUByteSeedValues UByteSeedValues {
            get {
                return this.uByteSeedValuesField;
            }
            set {
                this.uByteSeedValuesField = value;
            }
        }
        
        /// <remarks/>
        public RandoopPluginConfigSeedValuesShortSeedValues ShortSeedValues {
            get {
                return this.shortSeedValuesField;
            }
            set {
                this.shortSeedValuesField = value;
            }
        }
        
        /// <remarks/>
        public RandoopPluginConfigSeedValuesUShortSeedValues UShortSeedValues {
            get {
                return this.uShortSeedValuesField;
            }
            set {
                this.uShortSeedValuesField = value;
            }
        }
        
        /// <remarks/>
        public RandoopPluginConfigSeedValuesIntSeedValues IntSeedValues {
            get {
                return this.intSeedValuesField;
            }
            set {
                this.intSeedValuesField = value;
            }
        }
        
        /// <remarks/>
        public RandoopPluginConfigSeedValuesUIntSeedValues UIntSeedValues {
            get {
                return this.uIntSeedValuesField;
            }
            set {
                this.uIntSeedValuesField = value;
            }
        }
        
        /// <remarks/>
        public RandoopPluginConfigSeedValuesLongSeedValues LongSeedValues {
            get {
                return this.longSeedValuesField;
            }
            set {
                this.longSeedValuesField = value;
            }
        }
        
        /// <remarks/>
        public RandoopPluginConfigSeedValuesULongSeedValues ULongSeedValues {
            get {
                return this.uLongSeedValuesField;
            }
            set {
                this.uLongSeedValuesField = value;
            }
        }
        
        /// <remarks/>
        public RandoopPluginConfigSeedValuesDoubleSeedValues DoubleSeedValues {
            get {
                return this.doubleSeedValuesField;
            }
            set {
                this.doubleSeedValuesField = value;
            }
        }
        
        /// <remarks/>
        public RandoopPluginConfigSeedValuesFloatSeedValues FloatSeedValues {
            get {
                return this.floatSeedValuesField;
            }
            set {
                this.floatSeedValuesField = value;
            }
        }
        
        /// <remarks/>
        public RandoopPluginConfigSeedValuesDecimalSeedValues DecimalSeedValues {
            get {
                return this.decimalSeedValuesField;
            }
            set {
                this.decimalSeedValuesField = value;
            }
        }
        
        /// <remarks/>
        public RandoopPluginConfigSeedValuesBoolSeedValues BoolSeedValues {
            get {
                return this.boolSeedValuesField;
            }
            set {
                this.boolSeedValuesField = value;
            }
        }
        
        /// <remarks/>
        public RandoopPluginConfigSeedValuesStringSeedValues StringSeedValues {
            get {
                return this.stringSeedValuesField;
            }
            set {
                this.stringSeedValuesField = value;
            }
        }
        
        /// <remarks/>
        public RandoopPluginConfigSeedValuesCharSeedValues CharSeedValues {
            get {
                return this.charSeedValuesField;
            }
            set {
                this.charSeedValuesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    public partial class RandoopPluginConfigSeedValuesByteSeedValues {
        
        private sbyte[] valuesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Value", IsNullable=false)]
        public sbyte[] Values {
            get {
                return this.valuesField;
            }
            set {
                this.valuesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    public partial class RandoopPluginConfigSeedValuesUByteSeedValues {
        
        private byte[] valuesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute()]
        [System.Xml.Serialization.XmlArrayItemAttribute("Value", IsNullable=false)]
        public byte[] Values {
            get {
                return this.valuesField;
            }
            set {
                this.valuesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    public partial class RandoopPluginConfigSeedValuesShortSeedValues {
        
        private short[] valuesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Value", IsNullable=false)]
        public short[] Values {
            get {
                return this.valuesField;
            }
            set {
                this.valuesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    public partial class RandoopPluginConfigSeedValuesUShortSeedValues {
        
        private ushort[] valuesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Value", IsNullable=false)]
        public ushort[] Values {
            get {
                return this.valuesField;
            }
            set {
                this.valuesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    public partial class RandoopPluginConfigSeedValuesIntSeedValues {
        
        private int[] valuesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Value", IsNullable=false)]
        public int[] Values {
            get {
                return this.valuesField;
            }
            set {
                this.valuesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    public partial class RandoopPluginConfigSeedValuesUIntSeedValues {
        
        private uint[] valuesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Value", IsNullable=false)]
        public uint[] Values {
            get {
                return this.valuesField;
            }
            set {
                this.valuesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    public partial class RandoopPluginConfigSeedValuesLongSeedValues {
        
        private long[] valuesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Value", IsNullable=false)]
        public long[] Values {
            get {
                return this.valuesField;
            }
            set {
                this.valuesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    public partial class RandoopPluginConfigSeedValuesULongSeedValues {
        
        private ulong[] valuesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Value", IsNullable=false)]
        public ulong[] Values {
            get {
                return this.valuesField;
            }
            set {
                this.valuesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    public partial class RandoopPluginConfigSeedValuesDoubleSeedValues {
        
        private double[] valuesField;
        
        private bool usePositiveInfinityField;
        
        private bool useNegativeInfinityField;
        
        public RandoopPluginConfigSeedValuesDoubleSeedValues() {
            this.usePositiveInfinityField = false;
            this.useNegativeInfinityField = false;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Value", IsNullable=false)]
        public double[] Values {
            get {
                return this.valuesField;
            }
            set {
                this.valuesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool UsePositiveInfinity {
            get {
                return this.usePositiveInfinityField;
            }
            set {
                this.usePositiveInfinityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool UseNegativeInfinity {
            get {
                return this.useNegativeInfinityField;
            }
            set {
                this.useNegativeInfinityField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    public partial class RandoopPluginConfigSeedValuesFloatSeedValues {
        
        private float[] valuesField;
        
        private bool usePositiveInfinityField;
        
        private bool useNegativeInfinityField;
        
        public RandoopPluginConfigSeedValuesFloatSeedValues() {
            this.usePositiveInfinityField = false;
            this.useNegativeInfinityField = false;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Value", IsNullable=false)]
        public float[] Values {
            get {
                return this.valuesField;
            }
            set {
                this.valuesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool UsePositiveInfinity {
            get {
                return this.usePositiveInfinityField;
            }
            set {
                this.usePositiveInfinityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool UseNegativeInfinity {
            get {
                return this.useNegativeInfinityField;
            }
            set {
                this.useNegativeInfinityField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    public partial class RandoopPluginConfigSeedValuesDecimalSeedValues {
        
        private decimal[] valuesField;
        
        private bool useMinusOneField;
        
        public RandoopPluginConfigSeedValuesDecimalSeedValues() {
            this.useMinusOneField = false;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Value", IsNullable=false)]
        public decimal[] Values {
            get {
                return this.valuesField;
            }
            set {
                this.valuesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool UseMinusOne {
            get {
                return this.useMinusOneField;
            }
            set {
                this.useMinusOneField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    public partial class RandoopPluginConfigSeedValuesBoolSeedValues {
        
        private bool[] valuesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Value", IsNullable=false)]
        public bool[] Values {
            get {
                return this.valuesField;
            }
            set {
                this.valuesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    public partial class RandoopPluginConfigSeedValuesStringSeedValues {
        
        private string[] valuesField;
        
        private bool useEmptyStringField;
        
        private bool useNullField;
        
        public RandoopPluginConfigSeedValuesStringSeedValues() {
            this.useEmptyStringField = true;
            this.useNullField = true;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Value", IsNullable=false)]
        public string[] Values {
            get {
                return this.valuesField;
            }
            set {
                this.valuesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool UseEmptyString {
            get {
                return this.useEmptyStringField;
            }
            set {
                this.useEmptyStringField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool UseNull {
            get {
                return this.useNullField;
            }
            set {
                this.useNullField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://arktos.org/RandoopPluginConfig.xsd")]
    public partial class RandoopPluginConfigSeedValuesCharSeedValues {
        
        private string[] valuesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Value", IsNullable=false)]
        public string[] Values {
            get {
                return this.valuesField;
            }
            set {
                this.valuesField = value;
            }
        }
    }
}
