namespace Arktos.WinBert.VsPackage.ViewModel
{
    using Arktos.WinBert.Analysis;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Handles display of a successful analysis result.
    /// </summary>
    public class SuccessfulAnalysisVm : AnalysisVmBase
    {
        #region Constructors & Destructors

        public SuccessfulAnalysisVm(SuccessfulAnalysisResult result, string projectName)
            : base(projectName)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            this.Differences = this.TransformToBehavioralDiffs(result);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an observable collection of the differences presented by this vm.
        /// </summary>
        public ObservableCollection<BehavioralDifferenceVm> Differences { get; private set; }

        #endregion

        #region Private Methods

        private ObservableCollection<BehavioralDifferenceVm> TransformToBehavioralDiffs(SuccessfulAnalysisResult result)
        {
            var allMethodDiffs = result.Differences.SelectMany(test => test.MethodDifferences, (methodOwner, methodCall) => new { TestName = methodOwner.TestName, MethodCall = methodCall });
            var vms = allMethodDiffs.SelectMany(x => this.ProcessMethodCall(x.TestName, x.MethodCall));
            return new ObservableCollection<BehavioralDifferenceVm>(vms);
        }

        private IEnumerable<BehavioralDifferenceVm> ProcessMethodCall(string testName, MethodCallDifference methodDiff)
        {
            if (methodDiff.PostCallObjectDifferences.AreDifferences)
            {
                foreach (var diff in methodDiff.PostCallObjectDifferences.DiffStream)
                {
                    yield return new BehavioralDifferenceVm(
                        testName,
                        methodDiff.Signature,
                        methodDiff.Distance,
                        diff,
                        MethodCallDifferenceType.PostCallObject);
                }
            }

            if (methodDiff.ReturnValueDifference.AreDifferences)
            {
                if (methodDiff.ReturnValueDifference.IsPrimitive)
                {
                    yield return new BehavioralDifferenceVm(
                        testName,
                        methodDiff.Signature,
                        methodDiff.Distance,
                        methodDiff.ReturnValueDifference.PrimitiveDifference,
                        MethodCallDifferenceType.ReturnValue);
                }
                else if (methodDiff.ReturnValueDifference.IsObject)
                {
                    foreach (var diff in methodDiff.ReturnValueDifference.ObjectDifference.DiffStream)
                    {
                        yield return new BehavioralDifferenceVm(
                            testName,
                            methodDiff.Signature,
                            methodDiff.Distance,
                            diff,
                            MethodCallDifferenceType.ReturnValue);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Bad return value difference state!");
                }
            }
        }

        #endregion
    }
}
