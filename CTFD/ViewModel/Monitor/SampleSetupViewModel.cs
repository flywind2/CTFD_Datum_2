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
        public RelayCommand EditSample => new RelayCommand((o) =>
        {
            if (this.CurrentSample != null) this.EditCurreentSample(this.CurrentSample, false);
        });
    }
}
