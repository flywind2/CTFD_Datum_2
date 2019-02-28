using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTFD.Model.RuntimeData
{
    public class Patient:Base.Notify
    {
        private string name;
        private string sex;
        private string age;
        private string caseId;
        private string bedId;
        private string outPatientId;
        private string diagnosis;
        private string hospitalizationId;
        private string office;
        private string sampleId;
        private string samplingDate;
        private string sampleType;

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                this.RaisePropertyChanged(nameof(this.Name));
            }
        }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex
        {
            get => this.sex;
            set
            {
                this.sex = value;
                this.RaisePropertyChanged(nameof(this.Sex));
            }
        }

        /// <summary>
        /// 年龄
        /// </summary>
        public string Age
        {
            get => this.age;
            set
            {
                this.age = value;
                this.RaisePropertyChanged(nameof(this.Age));
            }
        }

        /// <summary>
        /// 病历号
        /// </summary>
        public string CaseId
        {
            get => this.caseId;
            set
            {
                this.caseId = value;
                this.RaisePropertyChanged(nameof(this.CaseId));
            }
        }
        
        /// <summary>
        /// 病床号
        /// </summary>
        public string BedId
        {
            get => this.bedId;
            set
            {
                this.bedId = value;
                this.RaisePropertyChanged(nameof(this.BedId));
            }
        }
        
        /// <summary>
        /// 门诊号
        /// </summary>
        public string OutPatientId
        {
            get => this.outPatientId;
            set
            {
                this.outPatientId = value;
                this.RaisePropertyChanged(nameof(this.OutPatientId));
            }
        }
        
        /// <summary>
        /// 临床诊断
        /// </summary>
        public string Diagnosis
        {
            get => this.diagnosis;
            set
            {
                this.diagnosis = value;
                this.RaisePropertyChanged(nameof(this.Diagnosis));
            }
        }
        
        /// <summary>
        /// 住院号
        /// </summary>
        public string HospitalizationId
        {
            get => this.hospitalizationId;
            set
            {
                this.hospitalizationId = value;
                this.RaisePropertyChanged(nameof(this.HospitalizationId));
            }
        }
       
        /// <summary>
        /// 送检科室
        /// </summary>
        public string Office
        {
            get => this.office;
            set
            {
                this.office = value;
                this.RaisePropertyChanged(nameof(this.Office));
            }
        }
       
        /// <summary>
        /// 样本号
        /// </summary>
        public string SampleId
        {
            get => this.sampleId;
            set
            {
                this.sampleId = value;
                this.RaisePropertyChanged(nameof(this.SampleId));
            }
        }
        
        /// <summary>
        /// 采样日期
        /// </summary>
        public string SamplingDate
        {
            get => this.samplingDate;
            set
            {
                this.samplingDate = value;
                this.RaisePropertyChanged(nameof(this.SamplingDate));
            }
        }
        
        /// <summary>
        /// 样本类型
        /// </summary>
        public string SampleType
        {
            get => this.sampleType;
            set
            {
                this.sampleType = value;
                this.RaisePropertyChanged(nameof(this.SampleType));
            }
        }
    }
}
