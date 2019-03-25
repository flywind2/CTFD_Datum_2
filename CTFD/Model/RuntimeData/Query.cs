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
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Account { get; set; } = "老王";

        [DataMember]
        public string StartDateTime { get; set; } = "201903250910";

        [DataMember]
        public string EndDateTime { get; set; } = "201903251210";

        public void Clear()
        {
           
        }
    }
}
