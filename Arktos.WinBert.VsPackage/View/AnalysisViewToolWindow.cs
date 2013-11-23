namespace Arktos.WinBert.VsPackage.View
{
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// Exposes our tool window to VS.
    /// </summary>
    [Guid("b792c760-b860-4962-9692-bd415921e7c6")]
    public class AnalysisViewToolWindow : ToolWindowPane
    {
        #region Fields & Constants

        private AnalysisView view;

        #endregion

        #region Constructors & Destructors

        public AnalysisViewToolWindow()
            : base(null)
        {
            this.Caption = Resources.ToolWindowTitle;
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;
            this.AnalysisView = new AnalysisView();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the analysis view for this tool window.
        /// </summary>
        public AnalysisView AnalysisView
        {
            get
            {
                return this.view;
            }

            set
            {
                this.view = value;
                base.Content = view;
            }
        }

        #endregion
    }
}
