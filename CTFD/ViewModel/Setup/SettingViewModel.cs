using CTFD.Global.Common;
using CTFD.Model.RuntimeData;
using CTFD.ViewModel.Base;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace CTFD.ViewModel.Setup
{
    public class SettingViewModel : Base.ViewModel
    {

        public Model.RuntimeData.Setup Configuration => General.WorkingData.Configuration;

        private bool isPopupColorSelectionBox;
        public bool IsPopupColorSelectionBox
        {
            get { return isPopupColorSelectionBox; }
            set
            {
                isPopupColorSelectionBox = value;
                this.RaisePropertyChanged(nameof(this.IsPopupColorSelectionBox));
            }
        }

        public RelayCommand AddDetection => new RelayCommand(() =>
        {
           
            this.WriteSetupFile();
        });

        public RelayCommand SelectColor => new RelayCommand(() => this.IsPopupColorSelectionBox = true);

        public RelayCommand ReConnectServer => new RelayCommand(() =>
        {
            General.RaiseGlobalHandler(GlobalEvent.ResetTcpClient);
            this.WriteSetupFile();
        });

        private void WriteSetupFile()
        {
            General.WriteJsonFile((object)Configuration, $"{Environment.CurrentDirectory}{Properties.Resources.SetupFilePath}", Encoding.Default);
        }
    }
}
