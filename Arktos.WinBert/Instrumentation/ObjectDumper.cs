namespace Arktos.WinBert.Instrumentation
{
    using System;

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
                    var type = target.GetType();
                    if (type.IsPrimitive)
                    {
                        throw new ArgumentException("Cannot dump a primitive type from this method!");
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    obj = (target == null) ? (Xml.Object)new Xml.Null() : (Xml.Object)new Xml.NotNull();
                }
            }

            return obj;
        }

        #endregion
    }
}
