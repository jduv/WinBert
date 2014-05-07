namespace Arktos.WinBert.Differencing
{
    using System.Collections.Generic;

    public interface IObjectDifference : IDifferenceResult
    {
        IEnumerable<PathedAnalysisLogDifference> DiffStream { get; }
        IEnumerable<ObjectDifference> FieldObjectDiffs { get; }
        IEnumerable<PathedAnalysisLogDifference> FieldPrimitiveDiffs { get; }
        IEnumerable<PathedAnalysisLogDifference> FieldSentinalDiffs { get; }
        IEnumerable<ObjectDifference> PropertyObjectDiffs { get; }
        IEnumerable<PathedAnalysisLogDifference> PropertyPrimitiveDiffs { get; }
        IEnumerable<PathedAnalysisLogDifference> PropertySentinalDiffs { get; }
    }
}
