using CTFD.Global.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTFD.ViewModel.Monitor
{
    public partial class MonitorViewModel
    {
        public void TransmitCtValue()
        {
            var data = new List<byte> { 0x16, 0x01 };
            data.AddRange (BitConverter.GetBytes(this.Experiment.Charts.Threshold));
            General.TcpClient.SendMsg(data.ToArray());
        }
    }
}
