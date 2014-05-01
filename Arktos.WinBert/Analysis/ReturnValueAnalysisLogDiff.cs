
namespace Arktos.WinBert.Differencing
{
    /// <summary>
    /// Represents a diff between return values. 
    /// </summary>
    public class ReturnValueAnalysisLogDiff : AnalysisLogDiff
    {
        #region Constructors & Destructors

        public ReturnValueAnalysisLogDiff(string oldValue, string newValue, string typeFullName)
            : base(oldValue, newValue, typeFullName)
        {
        }

        #endregion
    }
}
