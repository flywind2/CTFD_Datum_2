using CTFD.Global.Common;
using CTFD.Model.Base;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace CTFD.Model.RuntimeData
{
    [DataContract]
    public class Experiment : Notify
    {
        private Experiment backup;

        private int finalCurveIndex;

        private string name;

        [DataMember]
        public string StartTime { get; set; }

        private DateTime startTime;

        [DataMember]
        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
                this.RaisePropertyChanged(nameof(this.Name));
            }
        }

        [DataMember]
        public string User { get; set; }

        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public string ProjectName { get; set; }

        [DataMember]
        public Parameter Parameter { get; set; } = new Parameter();

        [DataMember]
        public Feedback[] Feedbacks { get; set; }

        [DataMember]
        public Sample[] Samples { get; set; }

        [IgnoreDataMember]
        public bool IsStarted { get; private set; }

        [IgnoreDataMember]
        public bool IsRealtimeFluorescenceSeries
        {
            get => this.RealtimeCurveIndex == 0 ? true : false;
            set
            {
                if (value)
                {
                    this.RealtimeCurveIndex = 0;
                    this.RaisePropertyChanged(nameof(this.RealtimeCurveIndex));
                    this.RaisePropertyChanged(nameof(this.IsRealtimeFluorescenceSeries));
                    this.RaisePropertyChanged(nameof(this.IsRealtimeTemperatureSeries));
                    this.RaisePropertyChanged(nameof(this.IsRealtimeMeltingSeries));
                    this.Charts.ChangeRealtimeCurve(this.RealtimeCurveIndex);
                }
            }
        }

        [IgnoreDataMember]
        public bool IsRealtimeMeltingSeries
        {
            get => this.RealtimeCurveIndex == 1 ? true : false;
            set
            {
                if (value)
                {
                    this.RealtimeCurveIndex = 1;
                    this.RaisePropertyChanged(nameof(this.RealtimeCurveIndex));
                    this.RaisePropertyChanged(nameof(this.IsRealtimeMeltingSeries));
                    this.RaisePropertyChanged(nameof(this.IsRealtimeFluorescenceSeries));
                    this.RaisePropertyChanged(nameof(this.IsRealtimeTemperatureSeries));
                    this.Charts.ChangeRealtimeCurve(this.RealtimeCurveIndex);
                }
            }
        }

        [IgnoreDataMember]
        public bool IsRealtimeTemperatureSeries
        {
            get => this.RealtimeCurveIndex == 2 ? true : false;
            set
            {
                if (value)
                {
                    this.RealtimeCurveIndex = 2;
                    this.RaisePropertyChanged(nameof(this.RealtimeCurveIndex));
                    this.RaisePropertyChanged(nameof(this.IsRealtimeTemperatureSeries));
                    this.RaisePropertyChanged(nameof(this.IsRealtimeFluorescenceSeries));
                    this.RaisePropertyChanged(nameof(this.IsRealtimeMeltingSeries));
                    this.Charts.ChangeRealtimeCurve(this.RealtimeCurveIndex);
                }
            }
        }

        [IgnoreDataMember]
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
                    this.Charts.ChangeFinalCurve(this.finalCurveIndex);
                }
            }
        }

        [IgnoreDataMember]
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
                    this.Charts.ChangeFinalCurve(this.finalCurveIndex);
                }
            }
        }

        [IgnoreDataMember]
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
                    this.Charts.ChangeFinalCurve(this.finalCurveIndex);
                }
            }
        }

        [IgnoreDataMember]
        public BackgroundTimer BackgroundTimer { get; private set; }

        [IgnoreDataMember]
        public BackgroundTimer CountdownTimer { get; private set; }

        [IgnoreDataMember]
        public Charts Charts { get; private set; }

        [IgnoreDataMember]
        public int RealtimeCurveIndex { get; private set; }

        public void Initialize()
        {
            this.Charts = new Charts();
            this.InitializeTimer();
            this.InitializeSamples();
            this.Backup();
            if (this.Feedbacks != null) this.Feedbacks[5].Value = "就绪";
        }

        private void InitializeSamples()
        {
            if (this.Samples != null)
            {
                for (int i = 0; i < this.Samples.Length; i++) Samples[i].InitializeSample(i);
            }
        }

        private void Backup()
        {
            this.backup = new Experiment();
            this.backup.GroupName = this.GroupName;
            this.backup.ProjectName = this.ProjectName;
            this.backup.Parameter = this.Parameter.GetCopy();
            this.backup.Samples = new Sample[32];
            if (this.Samples != null)
            {
                for (int i = 0; i < this.Samples.Length; i++) this.backup.Samples[i] = new Sample(this.Samples[i].ID, this.Samples[i].HoleName, this.Samples[i].Detection);
            }
        }

        private void InitializeTimer()
        {
            this.BackgroundTimer = new BackgroundTimer(1, 1, 1, 0, 0, 1, "HH:mm:ss", 1);
            this.CountdownTimer = new BackgroundTimer(1, 1, 1, 1, 0, 0, "HH:mm:ss", -1);
        }

        public int CalculateRemainningTime()
        {
            this.Parameter.HighSpeedTimes = 3;
            this.Parameter.LowSpeedTimes = 1;
            this.Parameter.MeltDuration = 1800;
            this.Parameter.CooldownDuration = 600;
            var remainningTime =
                this.Parameter.LysisDuration +
                this.Parameter.AmplificationDuration +
                (this.Parameter.LowSpeedDuration * this.Parameter.LowSpeedTimes) +
                (this.Parameter.HighSpeedDuration * this.Parameter.HighSpeedTimes) +
                this.Parameter.MeltDuration +
                this.Parameter.CooldownDuration;

            var hour = remainningTime / 3600;
            var minute = remainningTime % 3600 / 60;
            var second = remainningTime % 3600 % 60;
            this.CountdownTimer.ChangeTimer(new DateTime(1, 1, 1, hour, minute, second));
            return remainningTime / 60;
        }

        public void Start()
        {
            this.IsStarted = true;
            this.ReStartTimer();
            this.ResetExperiment();
        }

        public void Stop()
        {
            this.IsStarted = false;
            this.StopTimer();
        }

        private void ReStartTimer()
        {
            this.BackgroundTimer.Restart();
            this.CountdownTimer.Restart();
            this.startTime = DateTime.Now;
            //this.StartTime = this.startTime.ToString("yyyyMMddHHmmss");
        }

        private void StopTimer()
        {
            this.BackgroundTimer.Stop();
            this.CountdownTimer.Stop();
        }

        private void ResetExperiment()
        {
            this.Charts.InitializeSeries();
            this.Charts.SetSectionEnabled(false);
        }

        public void AddTemperatureValue(Token token, int value)
        {
            this.Charts.AddRealtimeTemperaturePoint(token, new ObservablePoint { X = DateTime.Now.Subtract(this.startTime).TotalMinutes, Y = value });
        }

        public void ChangeExperimentViewSeries(int index)
        {
            switch (index)
            {
                case 0:
                {

                    break;
                }
                case 1:
                {

                    break;
                }
                case 2:
                {

                    break;
                }
                default: break;
            }
        }

        public void SetStep(int step)
        {
            var currentStep = (Global.Common.Step)step;
            this.Feedbacks[5].Value = General.GetEnumDescription(currentStep);
            if (currentStep.Equals(Global.Common.Step.Compeleted))
            {

            }
        }

        public void SetRpm(short rpm)
        {
            this.Feedbacks[6].Value = rpm.ToString();
        }

        public void SetTemperature(Token token, short temperature)
        {
            var actualTemperature = ((double)temperature / 10).ToString("0.0");
            if (token == Token.UpperTemperature) this.Feedbacks[7].Value = actualTemperature;
            else if (token == Token.InnerRingTemperature) this.Feedbacks[8].Value = actualTemperature;
            else this.Feedbacks[9].Value = actualTemperature;
        }

        public void LoadSample(int index, bool isLoaded)
        {
            this.Samples[index].IsLoaded = isLoaded;
        }

        public void ChangeSeriesVisibility(Sample[] sampleIdentifications, bool isCurveDisplayed)
        {
            foreach (var item in sampleIdentifications)
            {
                this.Charts.ChangeSeriesVisibility(item.ID, isCurveDisplayed);
                this.Samples[item.ID].RaiseIsCurveDisplayed(isCurveDisplayed);
            }
            foreach (var item in this.Samples.Except(sampleIdentifications))
            {
                this.Charts.ChangeSeriesVisibility(item.ID, !isCurveDisplayed);
                this.Samples[item.ID].RaiseIsCurveDisplayed(!isCurveDisplayed);
            }
        }

        public void Rollback()
        {
            this.GroupName = this.backup.GroupName;
            this.ProjectName = this.backup.ProjectName;
            this.Parameter.Copy(this.backup.Parameter);
            for (int i = 0; i < this.Samples.Length; i++)
            {
                this.Samples[i].SetHoleName(this.backup.Samples[i].HoleName);
                this.Samples[i].SetDetection(this.backup.Samples[i].Detection);
            }
            this.RaisePropertyChanged(nameof(this.GroupName));
            this.RaisePropertyChanged(nameof(this.ProjectName));
            this.RaisePropertyChanged(nameof(this.Parameter));
            this.RaisePropertyChanged(nameof(this.Samples));
        }

        private void RaiseRealtimeXAxis(int ampTemperature, int ampXEnd, int experimentDuaring)
        {
            this.Charts.RaiseRealtimeXAxis(ampTemperature, ampXEnd, experimentDuaring);
            this.Charts.ChangeRealtimeCurve(this.RealtimeCurveIndex);
            this.Charts.ChangeFinalCurve(this.finalCurveIndex);
        }

        public void RaiseRealtimeXAxis()
        {
            if (this.IsStarted == false) this.RaiseRealtimeXAxis(this.Parameter.AmplificationTemperature, this.Parameter.GetTimeAxis(), this.CalculateRemainningTime());
        }

        public void ChangeSeriesVisibility(int sampleID, bool isCurveDisplayed) => this.Charts.ChangeSeriesVisibility(sampleID, isCurveDisplayed);

        public void AddRealtimeAmplificationValue(int[] values) => this.Charts.AddRealtimeAmplificationValue(values);

        public void AddRealtimeMeltingValue(int[] values) => this.Charts.AddRealtimeMeltingValue(values);

        public void AddAmplificationCurve(List<int[]> curveData) => this.Charts.AddFindAmplificationCurve(curveData);

        public void AddDerivationMeltingCurves(List<int[]> curveData) => this.Charts.AddDerivationMeltingCurves(curveData);

        public void AddStandardMeltingCurve(List<int[]> curveData = null) => this.Charts.AddStandardMeltingCurve(curveData);

        public void SetSectionEnabled(bool isEnabled) => this.Charts.SetSectionEnabled(isEnabled);

        public void SetStartTime() => this.StartTime = DateTime.Now.ToString("yyyyMMddHHmmss");

        public IEnumerable<int[]> GetAmplificationData(int curveIndex) => this.Charts.GetFinalCurveData(curveIndex);

        public bool GetFinalCurveStatus(int curveIndex) => this.Charts.GetFinalCurveStatus(curveIndex);

        public void Synchronous()
        {
            this.RaisePropertyChanged(nameof(this.Name));
            this.RaisePropertyChanged(nameof(this.User));
            this.RaisePropertyChanged(nameof(this.StartTime));
            this.Parameter.Synchronous();
        }
    }
}
