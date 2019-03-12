using CTFD.Model.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CTFD.Model.RuntimeData
{
    [DataContract]
    public class Setup : Notify
    {
        [DataMember]
        public Experiment Experiment { get; set; }

        [DataMember]
        public ObservableCollection<Account> Accounts { get; set; } = new ObservableCollection<Account>();

        [DataMember]
        public Account Account { get; set; } = new Account();

        [DataMember]
        public string CurrentTcpServerIPAddress { get; set; } = string.Empty;

        [DataMember]
        public int TcpServerPort { get; set; }

        public void RaiseConfiguration()
        {

        }
    }
}
