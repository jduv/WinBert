namespace Arktos.WinBert.Instrumentation
{
    using System;
    using Arktos.WinBert.Extensions;

    /// <summary>
    /// Logs objects of various types, converting them to a simple XML representation.
    /// </summary>
    public class ObjectDumper
    {
        #region Public Methods

        public Xml.Object DumpObject(object target, ushort maxDepth = 10)
        {
            Xml.Object obj;
            if (target == null)
            {
                obj = new Xml.Null();
            }
            else
            {
                if (maxDepth > 0)
                {
                    if (target.IsPrimitive())
                    {
                        throw new ArgumentException("Cannot dump a primitive type from this method!");
                    }
                    else
                    {
                        obj = new Xml.ReferenceType()
                        {
                            Type = target.GetType().FullName,
                            Field = DumpFields(target, maxDepth),
                            Property = DumpProperties(target, maxDepth)
                        };  
                    }
                }
                else
                {
                    obj = (target == null) ? (Xml.Object)new Xml.Null() : (Xml.Object)new Xml.NotNull();
                }
            }

            return obj;
        }

        public Xml.Primitive DumpPrimitive(object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            Xml.Primitive primitive;
            if (target.IsPrimitive())
            {
                primitive = new Xml.Primitive()
                {
                    Type = target.GetType().FullName,
                    Value = target.ToString()
                };
            }
            else
            {
                throw new ArgumentException("Cannot dump non-primitive type from this method!");
            }

            return primitive;
        }

        #endregion

        #region Private Methods

        Xml.Field[] DumpFields(object target, ushort maxDepth)
        {
            // BMK Implement me.
            return null;
        }

        Xml.Property[] DumpProperties(object target, ushort maxDepth)
        {
            // BMK Implement me.
            return null;
        }

        #endregion
    }
}
