using CTFD.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTFD.Model.Base
{
    public class BackgroundTimer : Notify
    {
        private readonly int interval = 1000;

        private readonly string timeFormat;

        private DateTime time;

        private DateTime initialTime;

        private DateTime targetTime = new DateTime(1, 1, 1, 0, 0, 0);

        private System.Timers.Timer timer;

        public event EventHandler Stopped;

        public string TimingData => this.time.ToString(this.timeFormat);

        public BackgroundTimer(int year, int month, int day, int hour, int minute, int second, string timeFormat, int scope)
        {
            this.time = new DateTime(year, month, day, hour, minute, second);
            this.initialTime = this.time;
            this.timeFormat = timeFormat;
            this.InitializeTimer(scope);
        }

        public BackgroundTimer(DateTime dateTime, string timeFormat, int scope)
        {
            this.time = dateTime;
            this.initialTime = this.time;
            this.timeFormat = timeFormat;
            this.InitializeTimer(scope);
        }

        public virtual void OnStopped()
        {
            this.Stopped?.Invoke(this, null);
        }

        private void InitializeTimer(int scope)
        {
            this.timer = new System.Timers.Timer(this.interval);
            this.timer.Elapsed += (sender, args) =>
            {
                App.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (this.time.Equals(this.targetTime)) this.Stop();
                    else
                    {
                        this.time = this.time.AddSeconds(scope);
                        this.RaisePropertyChanged(nameof(TimingData));
                    }
                });
            };
            this.timer.AutoReset = true;
        }

        public void ChangeTimer(DateTime dateTime)
        {

            this.time = dateTime;
            this.RaisePropertyChanged(nameof(TimingData));
        }

        public void Start()
        {
            this.timer.Enabled = true;
            this.timer.Start();
        }

       public void Restart()
        {
            this.time = this.initialTime;
            this.Start();
        }

        public void Stop()
        {
            this.timer.Stop();
            this.OnStopped();
        }
    }
}
