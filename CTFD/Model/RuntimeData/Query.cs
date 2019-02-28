using CTFD.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CTFD.Model.RuntimeData
{
    [DataContract]
    public class Query : Notify
    {
        private string name;
        [DataMember]
        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                this.RaisePropertyChanged(nameof(this.Name));
            }
        }

        private string account;
        [DataMember]
        public string Account
        {
            get => this.account;
            set
            {
                this.account = value;
                this.RaisePropertyChanged(nameof(this.Account));
            }
        }

        [DataMember]
        public string StartDateTime { get; set; }

        [DataMember]
        public string EndDateTime { get; set; }

        private string startMinute = "00";
        [IgnoreDataMember]
        public string StartMinute
        {
            get => this.startMinute;
            set
            {
                this.startMinute = value;
                this.RaiseValue();
            }
        }

        private string endMinute = "00";
        [IgnoreDataMember]
        public string EndMinute
        {
            get => this.endMinute;
            set
            {
                this.endMinute = value;
                this.RaiseValue();
            }
        }

        private string startHour = "00";
        [IgnoreDataMember]
        public string StartHour
        {
            get => this.startHour;
            set
            {
                this.startHour = value;
                this.RaiseValue();
            }
        }

        private string endHour = "00";
        [IgnoreDataMember]
        public string EndHour
        {
            get => this.endHour;
            set
            {
                this.endHour = value;
                this.RaiseValue();
            }
        }

        private string startDate = DateTime.Now.ToString("yyyy-MM-dd");
        [IgnoreDataMember]
        public DateTime? StartDate
        {
            get => Convert.ToDateTime(this.startDate);
            set
            {
                this.startDate = value.Value.ToString("yyyy-MM-dd");
                this.RaiseValue();
            }
        }

        private string endDate = DateTime.Now.ToString("yyyy-MM-dd");
        [IgnoreDataMember]
        public DateTime? EndDate
        {
            get => Convert.ToDateTime(this.endDate);
            set
            {
                this.endDate = value.Value.ToString("yyyy-MM-dd");
                this.RaiseValue();
            }
        }

        private void RaiseValue()
        {
            this.StartDateTime = $"{this.startDate} {this.startHour}:{this.startMinute}";
            this.EndDateTime = $"{this.endDate} {this.endHour}:{this.endMinute}";
        }
    }
}
