using CTFD.Model.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CTFD.Model.RuntimeData
{
    [DataContract]
    public class Parameter : Notify
    {
        /// <summary>
        /// 裂解温度
        /// </summary>
        [DataMember]
        public int LysisTemperature { get; set; }

        /// <summary>
        /// 裂解时间
        /// </summary>
        [DataMember]
        public int LysisDuration { get; set; }

        /// <summary>
        /// 扩增温度
        /// </summary>
        [DataMember]
        public int AmplificationTemperature { get; set; }

        /// <summary>
        /// 扩增时间
        /// </summary>
        [DataMember]
        public int AmplificationDuration { get; set; }

        /// <summary>
        /// 低转速
        /// </summary>
        [DataMember]
        public int LowSpeed { get; set; } = 1600;

        /// <summary>
        /// 低速时间
        /// </summary>
        [DataMember]
        public int LowSpeedDuration { get; set; } = 10;

        /// <summary>
        /// 低速离心次数
        /// </summary>
        [IgnoreDataMember]
        public int LowSpeedTimes { get; set; } = 1;

        /// <summary>
        /// 高转速
        /// </summary>
        [DataMember]
        public int HighSpeed { get; set; } = 4600;

        /// <summary>
        /// 高速时间
        /// </summary>
        [DataMember]
        public int HighSpeedDuration { get; set; } = 30;

        [IgnoreDataMember]
        public int HighSpeedTimes { get; set; } = 3;

        /// <summary>
        /// 熔解时间
        /// </summary>
        [DataMember]
        public int MeltDuration { get; set; }

        [IgnoreDataMember]
        public int CooldownDuration { get; set; } = 30;

        /// <summary>
        /// 是否熔解
        /// </summary>
        [DataMember]
        public bool IsMelt { get; set; }

        public Parameter GetCopy()
        {
            return new Parameter
            {
                AmplificationTemperature = this.AmplificationTemperature,
                AmplificationDuration = this.AmplificationDuration,
                HighSpeed = this.HighSpeed,
                LowSpeed = this.LowSpeed,
                HighSpeedDuration = this.HighSpeedDuration,
                IsMelt = this.IsMelt,
                LowSpeedDuration = this.LowSpeedDuration,
                LysisDuration = this.LysisDuration,
                LysisTemperature = this.LysisTemperature,
                MeltDuration = this.MeltDuration
            };
        }

        public void Copy(Parameter parameter)
        {
            this.AmplificationTemperature = parameter.AmplificationTemperature;
            this.AmplificationDuration = parameter.AmplificationDuration;
            this.HighSpeed = parameter.HighSpeed;
            this.LowSpeed = parameter.LowSpeed;
            this.HighSpeedDuration = parameter.HighSpeedDuration;
            this.IsMelt = parameter.IsMelt;
            this.LowSpeedDuration = parameter.LowSpeedDuration;
            this.LysisDuration = parameter.LysisDuration;
            this.LysisTemperature = parameter.LysisTemperature;
            this.MeltDuration = parameter.MeltDuration;
        }

        public int GetTimeAxis() => (this.AmplificationDuration / 60+2) * 2;

        public void RaisePropertyChanged(int index)
        {
            switch (index)
            {
                case 0: { this.RaisePropertyChanged(nameof(LysisTemperature)); break; }
                default: break;
            }
        }
    }
}
