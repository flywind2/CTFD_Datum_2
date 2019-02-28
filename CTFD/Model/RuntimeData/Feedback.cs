using CTFD.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CTFD.Model.RuntimeData
{
    [DataContract]
    public class Feedback:Notify
    {
        private Visibility status=Visibility.Visible;
        [DataMember]
        public Visibility Status
        {
            get { return this.status; }
            set
            {
                this.status = value;
                this.RaisePropertyChanged(nameof(this.Status));
            }
        }

        private string value;
        public string Value
        {
            get { return this.value; }
            set
            {
                this.value = value;
                this.RaisePropertyChanged(nameof(this.Value));
            }
        }

    }
}
