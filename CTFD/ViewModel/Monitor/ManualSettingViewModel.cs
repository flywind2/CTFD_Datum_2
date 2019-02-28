using CTFD.Global.Common;
using CTFD.Model.RuntimeData;
using CTFD.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTFD.ViewModel.Monitor
{
    public class ManualSettingViewModel : Base.ViewModel, ISample
    {
        public Experiment Experiment { get; private set; }

        public int AT
        {
            get => this.Experiment.Parameter.AmplificationTemperature / 10;
            set
            {
                if (int.TryParse(value.ToString(), out int result))
                {
                    if (result > 0 && result < 100)
                    {
                        this.Experiment.Parameter.AmplificationTemperature = result * 10;
                        this.RaisePropertyChanged(nameof(this.AT));
                    }
                }
            }
        }

        public int AD
        {
            get => this.Experiment.Parameter.AmplificationDuration / 60;
            set
            {
                if (int.TryParse(value.ToString(), out int result))
                {
                    if (result > 0 && result < 100)
                    {
                        this.Experiment.Parameter.AmplificationDuration = value * 60;
                        this.RaisePropertyChanged(nameof(this.AD));
                    }
                }
            }
        }

        private string holeName;

        public string HoleName
        {
            get { return this.holeName; }
            set
            {
                this.holeName = value;
                foreach (var item in this.Experiment.Samples.Where(o => o.IsSelected == true))
                {
                    ((Sample)item).SetHoleName(value);
                }
                //this.RaisePropertyChanged(nameof(this.HoleName));
            }
        }


        public RelayCommand Close => new RelayCommand(() => this.OnViewChanged());

        public RelayCommand Reset => new RelayCommand(() => this.Experiment.Samples[0].SetHoleName("WAH"));

        public ManualSettingViewModel(Experiment experiment)
        {
            this.Experiment = experiment;
            //this.Experiment.InitializeExperiment();
            this.RaisePropertyChanged(nameof(this.Experiment));
        }

        void ISample.ResetSelection()
        {
            foreach (var item in this.Experiment.Samples) item.IsSelected = false;
        }
    }
}
