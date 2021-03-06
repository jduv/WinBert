﻿
namespace Arktos.WinBert.VsPackage.ViewModel
{
    using System;

    /// <summary>
    /// Handles displaying error information to the user.
    /// </summary>
    public class AnalysisErrorInfoVm : AnalysisVmBase
    {
        #region Fields & Constants

        private string message;
        private Exception exception;

        #endregion

        #region Constructors & Destructors

        public AnalysisErrorInfoVm(string message, string projectName)
            : base(projectName)
        {
            this.ErrorInfo = message;
        }

        public AnalysisErrorInfoVm(Exception exception, string projectName)
            : base(projectName)
        {
            this.ErrorInfo = exception.Message;
            this.Exception = exception;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets some information about the error to display to the user.
        /// </summary>
        public string ErrorInfo
        {
            get
            {
                return this.message;
            }

            private set
            {
                base.Set("ErrorInfo", ref this.message, value);
            }
        }

        /// <summary>
        /// Gets an exception, if available, that was thrown to cause the error.
        /// </summary>
        public Exception Exception
        {
            get
            {
                return this.exception;
            }

            private set
            {
                base.Set("Exception", ref this.exception, value);
            }
        }

        #endregion
    }
}
