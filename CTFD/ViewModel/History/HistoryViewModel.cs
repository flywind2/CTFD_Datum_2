using CTFD.Global.Common;
using CTFD.Model.RuntimeData;
using CTFD.ViewModel.Base;
using LiveCharts;
using LiveCharts.Geared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CTFD.ViewModel.History
{
    public class HistoryViewModel : Base.ViewModel
    {
        private int finalCurveIndex;

        public Experiment Experiment { get; private set; }

        public List<Experiment> Experiments { get; set; }

        public Query Query { get; set; } = new Query();

        public bool IsAmplificationSeries
        {
            get => this.finalCurveIndex == 0 ? true : false;
            set
            {
                if (value)
                {
                    this.finalCurveIndex = 0;
                    this.RaisePropertyChanged(nameof(this.IsMelting1Series));
                    this.RaisePropertyChanged(nameof(this.IsMelting2Series));
                    this.Experiment.Charts.ChangeFinalCurve(this.finalCurveIndex);
                }
            }
        }

        public bool IsMelting1Series
        {
            get => this.finalCurveIndex == 1 ? true : false;
            set
            {
                if (value)
                {
                    this.finalCurveIndex = 1;
                    this.RaisePropertyChanged(nameof(this.IsMelting2Series));
                    this.RaisePropertyChanged(nameof(this.IsAmplificationSeries));
                    this.Experiment.Charts.ChangeFinalCurve(this.finalCurveIndex);
                }
            }
        }

        public bool IsMelting2Series
        {
            get => this.finalCurveIndex == 2 ? true : false;
            set
            {
                if (value)
                {
                    this.finalCurveIndex = 2;
                    this.RaisePropertyChanged(nameof(this.IsMelting1Series));
                    this.RaisePropertyChanged(nameof(this.IsAmplificationSeries));
                    this.Experiment.Charts.ChangeFinalCurve(this.finalCurveIndex);
                }
            }
        }

        public RelayCommand Search => new RelayCommand(() =>
        {
            var result = new List<byte> { 9, 1 };
            result.AddRange(General.JsonSerialize(this.Query));
            General.TcpClient.SendMsg(result.ToArray());
        });

        public RelayCommand<Experiment> SearchDetail => new RelayCommand<Experiment>((parameter) =>
        {
            this.Experiment.Name = parameter.Name;
            this.Experiment.User = parameter.User;
            this.Experiment.StartTime = parameter.StartTime;
            this.Experiment.Parameter = parameter.Parameter;
            var result = new List<byte> { 11, 1 };
            result.AddRange(General.JsonSerialize(parameter.StartTime));
            General.TcpClient.SendMsg(result.ToArray());
        });

        public HistoryViewModel()
        {
            General.GlobalHandler += General_GlobalHandler;
            this.Experiment = new Experiment();
            this.Experiment.Initialize();
        }

        private void General_GlobalHandler(object sender, GlobalEventArgs e)
        {
            switch (e.GlobalEvent)
            {
                case GlobalEvent.Query:
                {
                    var experiments = General.JsonDeserialize<List<Experiment>>(e.Value as byte[]);
                    experiments[0].Parameter.AmplificationTemperature = 680;
                    this.Experiments = new List<Experiment>(experiments);
                    this.RaisePropertyChanged(nameof(this.Experiments));
                    break;
                }
                case GlobalEvent.HistoryCurve1:
                {
                    var amplificationCurve = General.JsonDeserialize<List<int[]>>(e.Value as byte[]);
                    this.Experiment.Charts.AddFindAmplificationCurve(amplificationCurve);
                    break;
                }
                case GlobalEvent.HistoryCurve2:
                {
                    var standardMeltingCurve = General.JsonDeserialize<List<int[]>>(e.Value as byte[]);
                    this.Experiment.Charts.AddStandardMeltingCurve(standardMeltingCurve);
                    break;
                }
                case GlobalEvent.HistoryCurve3:
                {
                    var derivationMelting = General.JsonDeserialize<List<int[]>>(e.Value as byte[]);
                    this.Experiment.Charts.AddDerivationMeltingCurves(derivationMelting);
                    //this.Experiment.Synchronous();
                    this.RaisePropertyChanged(nameof(this.Experiment));
                    break;
                }
                default: break;
            }
        }

        private SeriesCollection CreateSeries<T>(int seriesCount, int smoothness = 3)
        {
            var result = new SeriesCollection();
            var lines = new GLineSeries[seriesCount];
            for (int i = 0; i < seriesCount; i++) lines[i] = new GLineSeries { Fill = Brushes.Transparent, StrokeThickness = 1, LineSmoothness = smoothness, Values = new ChartValues<T>().AsGearedValues().WithQuality(Quality.Highest), PointGeometrySize = 0 };
            result.AddRange(lines);
            return result;
        }

        private List<Experiment> TestData()
        {
            var result = new List<Experiment>();
            //result.Add(new Experiment { Name = "测试实验1", User = "老王", StartTime = "201903250910" });
            return result;
        }
    }
}

