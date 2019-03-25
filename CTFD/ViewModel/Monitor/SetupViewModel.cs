using CTFD.Global.Common;
using CTFD.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTFD.ViewModel.Monitor
{
    public partial class MonitorViewModel
    {
        public RelayCommand ReConnect => new RelayCommand(() => General.RaiseGlobalHandler(GlobalEvent.ResetTcpClient));

        public RelayCommand ApplySetup => new RelayCommand(() => this.WriteSetupFile());
       
        public void ResetTcpClient() => this.InitializeTcpClient(this.Configuration.CurrentTcpServerIPAddress, this.Configuration.TcpServerPort);
    }
}
