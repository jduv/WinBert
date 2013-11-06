namespace Arktos.WinBert.Instrumentation
{
    using System;

    /// <summary>
    /// Represents an implementation that may consume generic objects and produce the WinBert analysis
    /// log schema. 
    /// </summary>
    public interface IObjectDumper
    {
        #region Methods

        /// <summary>
        /// In essence, this is a factory method for creating Xml.Object instances. This only works for non-primitive
        /// objects and structs.
        /// </summary>
        /// <param name="target">
        /// The target to dump.
        /// </param>
        /// <param name="maxDepth">
        /// The maximum depth to go when recursively logging instance fields and properties. Defaults to 3.
        /// </param>
        /// <returns>
        /// An Xml.Object instance representing the passed in target.
        /// </returns>
        Xml.Object DumpObject(object target, ushort maxDepth = 3);

        /// <summary>
        /// Creates Xml.Primitive instances. This only works for objects that are considered to be
        /// "primtive," which isn't precisly correlated to the obj.GetType().IsPrimitive property. There
        /// are a couple of other CLR types that can be considered primitives for our purposes here
        /// even if there are no IL instructions that operate on them (which is the CLR's definition of
        /// a primitive as of now).
        /// </summary>
        /// <param name="target">
        /// The target primitive object to dump.
        /// </param>
        /// <returns>
        /// An Xml.Primitive object representing the passed in target.
        /// </returns>
        Xml.Primitive DumpPrimitive(object target);

        #endregion

    }
}
