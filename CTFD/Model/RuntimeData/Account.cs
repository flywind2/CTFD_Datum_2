using CTFD.Global.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CTFD.Model.RuntimeData
{
    [DataContract]
    public class Account: Base.Notify
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string Role { get; set; }

        [DataMember]
        public string DateTime { get; set; }

        public void RaiseProperties()
        {
           this.RaisePropertyChanged(nameof(this.UserName));
        }

        public Account() { }

        public Account(string userName, string password, string role)
        {
            this.UserName = userName;
            this.Password = password;
            this.Role = role;
            this.DateTime = System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }

        public void Clear()
        {
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.Role = string.Empty;
            this.RaisePropertyChanged(nameof(this.UserName));
            this.RaisePropertyChanged(nameof(this.Password));
            this.RaisePropertyChanged(nameof(this.Role));
        }
    }
}
