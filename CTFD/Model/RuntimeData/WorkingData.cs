using CTFD.Model.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTFD.Model.RuntimeData
{
    public class WorkingData : Notify
    {
        public Setup Configuration { get; set; } = new Setup();

        //public Experiment DefaultExperiment { get; set; }

        public Query Query { get; set; } = new Query();

        private List<Query> results = new List<Query>();
        public List<Query> Results
        {
            get => this.results;
            set
            {
                this.results = value;
                this.RaisePropertyChanged(nameof(this.Results));
            }
        }

        public bool IsConnected { get; set; }

        public WorkingData()
        {

        }
    }
}
