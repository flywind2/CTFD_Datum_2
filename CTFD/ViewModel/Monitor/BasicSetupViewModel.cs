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
        public RelayCommand Rollback => new RelayCommand(() =>
        {
            this.Experiment.Rollback();
            this.RaisePropertyChanged(nameof(this.AD));
            this.RaisePropertyChanged(nameof(this.AT));
            this.RaisePropertyChanged(nameof(this.DD));
            this.RaisePropertyChanged(nameof(this.DT));
            this.holeName = string.Empty;
            this.RaisePropertyChanged(nameof(this.HoleName));
            this.detection = string.Empty;
            this.RaisePropertyChanged(nameof(this.Detection));
            //(ISample)
        });
    }
}
