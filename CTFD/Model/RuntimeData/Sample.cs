using CTFD.Global;
using CTFD.Global.Common;
using CTFD.Model.Base;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CTFD.Model.RuntimeData
{
    [DataContract]
    public class Sample : Notify
    {
        private string name = string.Empty;
        private readonly Brush brush;
        private bool isSelected;
        private bool isLoaded;
        private bool isCurveDisplayed = true;
        private string ctResult = string.Empty;
        private string tmResult = string.Empty;

        [IgnoreDataMember]
        public Patient Patient { get; set; }

        [IgnoreDataMember]
        public LineSeries RawSeries { get; set; }

        [IgnoreDataMember]
        public LineSeries SmoothSeries { get; set; }

        [IgnoreDataMember]
        public LineSeries AmplificationSeries { get; set; }

        [IgnoreDataMember]
        public LineSeries MeltingSeries { get; set; }

        [IgnoreDataMember]
        public int ID { get; set; }

        [DataMember]
        public string HoleName { get; private set; }

        [IgnoreDataMember]
        public bool IsLoaded
        {
            get => this.isLoaded;
            set
            {
                this.isLoaded = value;
                this.RaisePropertyChanged(nameof(this.IsLoaded));
            }
        }

        [DataMember]
        public string Detection { get; private set; }

        [IgnoreDataMember]
        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
                this.RaisePropertyChanged(nameof(this.Name));
            }
        }

        [IgnoreDataMember]
        public string Type { get; set; }

        [IgnoreDataMember]
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                //this.RaiseLineStatus(value);
                this.RaisePropertyChanged(nameof(this.IsSelected));
            }
        }

        [IgnoreDataMember]
        public bool IsCurveDisplayed
        {
            get => this.isCurveDisplayed;
            set
            {
                this.isCurveDisplayed = value;
                General.RaiseGlobalHandler(GlobalEvent.CurveVisibilityChanged, new Tuple<int, bool>(this.ID, value));
            }
        }

        [IgnoreDataMember]
        public string CtResult
        {
            get => this.ctResult;
            set
            {
                this.ctResult = value;
                this.RaisePropertyChanged(nameof(this.CtResult));
            }
        }

        [IgnoreDataMember]
        public string TmResult
        {
            get => this.tmResult;
            set
            {
                this.tmResult = value;
                this.RaisePropertyChanged(nameof(this.TmResult));
            }
        }

        public Sample(int id, string holeName, string detection)
        {
            this.ID = id;
            this.HoleName = holeName ?? string.Empty;
            this.Detection = detection ?? string.Empty;
            this.Patient = new Patient();
        }

        public Sample() : this(0, string.Empty, string.Empty) { }

        public void SetHoleName(string holeName = null)
        {
            this.HoleName = holeName ?? string.Empty;
            this.RaisePropertyChanged(nameof(this.HoleName));
        }

        public void SetDetection(string detection = null)
        {
            this.Detection = detection ?? string.Empty;
            this.RaisePropertyChanged(nameof(this.Detection));
        }

        public void InitializeSample(int id)
        {
            this.Patient = new Patient();
            this.ID = id;
            this.CtResult = string.Empty;
            this.TmResult = string.Empty;
            this.isCurveDisplayed = true;
            this.RaisePropertyChanged(nameof(this.IsCurveDisplayed));
        }

        public void RaiseIsCurveDisplayed(bool isCurveDisplayed)
        {
            this.isCurveDisplayed = isCurveDisplayed;
            this.RaisePropertyChanged(nameof(this.isCurveDisplayed));
        }
    }
}
