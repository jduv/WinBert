﻿namespace Arktos.WinBert.VsPackage
{
    using Arktos.WinBert.VsPackage.View;
    using EnvDTE80;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.ComponentModel.Design;
    using System.Runtime.InteropServices;

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(AnalysisViewToolWindow))]
    [Guid(GuidList.GuidWinBertVsPackagePkgString)]
    [ProvideAutoLoad(GuidList.GuidUiContextAnySolution)]
    public sealed class WinBertVsPackage : Package
    {
        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the WinBertVsPackage class.
        ///   Inside this method you can place any initialization code that does not require 
        ///   any Visual Studio service because at this point the package object is created but 
        ///   not sited yet inside Visual Studio environment. The place to do all the other 
        ///   initialization is the Initialize method.
        /// </summary>
        public WinBertVsPackage()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an instance of the WinBertServiceProvider for this package.
        /// </summary>
        public WinBertServiceProvider WinBertServiceProvider
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the tool window for the analysis engine. If one doesn't exist, it will create it. 
        /// </summary>
        public AnalysisViewToolWindow AnalysisWindow
        {
            get
            {
                return this.FindToolWindow(typeof(AnalysisViewToolWindow), 0, true) as AnalysisViewToolWindow;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = this.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (mcs != null)
            {
                // Create the command for the tool window
                CommandID toolwndCommandID = new CommandID(
                    GuidList.GuidWinBertVsPackageCmdSet,
                    (int)PkgCmdIDList.CmdIdBertAnalysisWindow);
                MenuCommand menuToolWin = new MenuCommand(ShowToolWindow, toolwndCommandID);
                mcs.AddCommand(menuToolWin);
            }

            // Grab the DTE2 instance and pass it to the WinBertServiceProvider
            DTE2 dte2 = ServiceProvider.GlobalProvider.GetService(typeof(SDTE)) as EnvDTE80.DTE2;
            if (dte2 != null)
            {
                this.WinBertServiceProvider = new WinBertServiceProvider(dte2);
            }

            // Attempt to site the diata context for the analysis window.
            var window = this.AnalysisWindow;
            if (window != null)
            {
                window.AnalysisView.DataContext = this.WinBertServiceProvider.AnalysisVm;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This function is called when the user clicks the menu item that shows the 
        /// tool window. See the Initialize method to see how the menu item is associated to 
        /// this function using the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void ShowToolWindow(object sender, EventArgs e)
        {
            var window = this.AnalysisWindow;
            if (window == null || window.Frame == null)
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }

            if (this.WinBertServiceProvider == null)
            {
                throw new NotSupportedException(Resources.ExtensionNotLoaded);
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        #endregion
    }
}
