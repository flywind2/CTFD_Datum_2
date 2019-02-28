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
        public Role Role { get; set; }

        [DataMember]
        public string DateTime { get; set; }

        [DataMember]
        public string Remark { get; set; }

        public void RaiseProperties()
        {
           this.RaisePropertyChanged(nameof(this.UserName));
        }

        public Account() { }

        public Account(string userName, string password, Role role, string remark)
        {
            this.UserName = userName;
            this.Password = password;
            this.Role = role;
            this.DateTime = System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            this.Remark = remark;
        }
    }
}
