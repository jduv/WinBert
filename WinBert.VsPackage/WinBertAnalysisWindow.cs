namespace WinBert.VsPackage
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control. In Visual Studio tool
    ///   windows are composed of a frame (implemented by the shell) and a pane, usually implemented by the package 
    ///   implementer. This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    ///   implementation of the IVsUIElementPane interface.
    /// </summary>
    [Guid("b792c760-b860-4962-9692-bd415921e7c6")]
    public class WinBertAnalysisWindow : ToolWindowPane
    {
        #region Constants and Fields

        /// <summary>
        ///   There ever only needs to be a single instance of the AnalysisView.
        /// </summary>
        private AnalysisView analysisView = new AnalysisView();

        #endregion

        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the WinBertAnalysisWindow class.
        /// </summary>
        public WinBertAnalysisWindow() : base(null)
        {
            this.Caption = Resources.ToolWindowTitle;
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;

            this.Content = this.analysisView;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the DataContext for the analysis window's child WPF control.
        /// </summary>
        public IWinBertServiceProvider DataContext
        {
            get
            {
                return this.analysisView.DataContext as IWinBertServiceProvider;
            }

            set
            {
                this.analysisView.DataContext = value;
            }
        }

        #endregion
    }
}
