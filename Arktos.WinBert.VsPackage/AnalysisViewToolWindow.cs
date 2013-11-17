namespace Arktos.WinBert.VsPackage
{
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// Exposes our tool window to VS.
    /// </summary>
    [Guid("b792c760-b860-4962-9692-bd415921e7c6")]
    public class AnalysisViewToolWindow : ToolWindowPane
    {
        #region Constructors & Destructors

        public AnalysisViewToolWindow() : base(null)
        {
            this.Caption = Resources.ToolWindowTitle;
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;
            base.Content = new AnalysisView();
        }

        #endregion
    }
}
