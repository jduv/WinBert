using System.Collections.Generic;
using WinBert.Analysis;
namespace WinBert.VsPackage.ViewModels
{
    public class WinBertServiceViewModel : ViewModelBase
    {
        #region Fields

        IWinBertServiceProvider winBertServiceProvider = null;

        #endregion

        #region Constructors

        public WinBertServiceViewModel(IWinBertServiceProvider serviceProvider)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the IWinBertServiceProvider instance.
        /// </summary>
        public IWinBertServiceProvider WinBertServiceProvider
        {
            get
            {
                return this.winBertServiceProvider;
            }

            set
            {
                this.winBertServiceProvider = value;
                this.RaisePropertyChanged("WinBertServiceProvider");
            }
        }

        #endregion
    }
}
