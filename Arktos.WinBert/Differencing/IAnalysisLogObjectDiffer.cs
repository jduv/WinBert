namespace Arktos.WinBert.Differencing
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines an interface for an object that can perform differences between analysis log elements.
    /// </summary>
    interface IAnalysisLogObjectDiffer
    {
        #region Methods

        /// <summary>
        /// Performs a deep difference between the two target analysis log objects.
        /// </summary>
        /// <param name="oldObject">
        /// The old object.
        /// </param>
        /// <param name="newObject">
        /// The new object.
        /// </param>
        /// <returns>
        /// A stream of log diffs. This stream will be empty if no differences can be found.
        /// </returns>
        IEnumerable<IAnalysisLogDiff> DiffObjects(Xml.Object oldObject, Xml.Object newObject);

        /// <summary>
        /// Performs a deep difference of the two target analysis log value objects.
        /// </summary>
        /// <param name="oldValue">
        ///  The old value.
        /// </param>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        /// <returns>
        /// A stream of log diffs. This stream will be empty if no differences can be found.
        /// </returns>
        IEnumerable<IAnalysisLogDiff> DiffValues(Xml.Value oldValue, Xml.Value newValue);

        /// <summary>
        /// Performs a difference between the two target primitives.
        /// </summary>
        /// <param name="oldPrimitive">
        /// The old primitive.
        /// </param>
        /// <param name="newPrimitive">
        /// The new primitive.
        /// </param>
        /// <returns>
        /// A stream of log diffs. In order to remain consistent across the other methods we picked an IEnumerable even
        /// though this method every only returns a set with one element. This enables simpler cascades of sequences 
        /// when computing differences in deep object graphs. The set will be empty if no diffs exist.
        /// </returns>
        IEnumerable<IAnalysisLogDiff> DiffPrimitives(Xml.Primitive oldPrimitive, Xml.Primitive newPrimitive);

        #endregion
    }
}
