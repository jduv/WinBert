namespace WinBert.VsPackage.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    /// <summary>
    /// A very simple abstract VM base class. We don't need anything fancy here--just the
    /// basics.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Events

        /// <summary>
        /// The property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating that the VM should throw on an invalid
        /// property name.
        /// </summary>
        public bool ThrowOnInvalidPropertyName
        {
            get;
            set;
        }
        
        #endregion

        #region Protected Methods

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Verifies that the target property name exists. If it doesn't exist, then it will
        /// either log a message to the console or throw an exception.
        /// </summary>
        /// <param name="propertyName">The property name to check for.</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        protected void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;
                if (this.ThrowOnInvalidPropertyName)
                {
                    throw new Exception(msg);
                }
                else
                {
                    Debug.Fail(msg);
                }
            }
        }

        #endregion
    }
}
