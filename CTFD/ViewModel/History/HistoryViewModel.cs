using CTFD.Global.Common;
using CTFD.Model.RuntimeData;
using CTFD.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTFD.ViewModel.History
{
    public class HistoryViewModel : Base.ViewModel
    {
        public ObservableCollection<Experiment> Experiments { get; set; }

        public Query Query { get; set; } = new Query();

        public RelayCommand Search => new RelayCommand(() =>
        {
            var result = new List<byte> { 9, 1 };
            result.AddRange(General.JsonSerialize(this.Query));
            General.TcpClient.SendMsg(result.ToArray());
        });

        public RelayCommand Search2 => new RelayCommand(()=> 
        {
            var result = new List<byte> { 11, 1 };
            result.AddRange(General.JsonSerialize(this.Query.EndDateTime));
            General.TcpClient.SendMsg(result.ToArray());
        });


        public HistoryViewModel()
        {
            General.GlobalHandler += General_GlobalHandler;
        }

        private void General_GlobalHandler(object sender, GlobalEventArgs e)
        {
            switch (e.GlobalEvent)
            {
                case GlobalEvent.Query:
                {
                    var experiments = General.JsonDeserialize<List<Experiment>>(e.Value as byte[]);
                    this.Experiments = new ObservableCollection<Experiment>(experiments);
                    this.RaisePropertyChanged(nameof(this.Experiments));
                    break;
                }
                case GlobalEvent.HistoryCurve1:
                {
                    var aaa = e.Value as byte[];
                    break;
                }
                case GlobalEvent.HistoryCurve2:
                {
                    var aaa = e.Value as byte[];
                    break;
                }
                case GlobalEvent.HistoryCurve3:
                {
                    var aaa = e.Value as byte[];
                    break;
                }
                default: break;
            }

        }
    }
}

