using Arktos.WinBert.Analysis;

namespace Arktos.WinBert.VsPackage.ViewModel
{
    public enum MethodCallDifferenceType
    {
        PostCallObject,
        ReturnValue
    }

    public class BehavioralDifferenceVm
    {
        public BehavioralDifferenceVm(string testName, string methodSig, int? distance, AnalysisLogDifference diff, MethodCallDifferenceType diffType)
        {
            var pathedDiff = diff as PathedAnalysisLogDifference;
            this.TestName = testName;
            this.MethodSignature = methodSig;
            this.Distance = distance;
            this.DiffType = diffType;
            this.MemberPath = pathedDiff != null ? pathedDiff.Path : null;
            this.PreviousValue = diff.OldValue;
            this.CurrentValue = diff.NewValue;
            this.ValueTypeFullName = diff.TypeFullName;
        }
        public string TestName { get; private set; }
        public string MethodSignature { get; private set; }
        public int? Distance { get; private set; }
        public MethodCallDifferenceType DiffType { get; private set; }
        public string MemberPath { get; private set; }
        public string ValueTypeFullName { get; private set; }
        public string PreviousValue { get; private set; }
        public string CurrentValue { get; private set; }
    }
}
