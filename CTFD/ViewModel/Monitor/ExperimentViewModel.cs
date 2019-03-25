using CTFD.Global.Common;
using CTFD.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTFD.ViewModel.Monitor
{
    public partial class MonitorViewModel
    {
        public RelayCommand RunOrPause => new RelayCommand(() =>
        {
            switch (General.Status)
            {
                case Status.Stop:
                {
                    Task.Factory.StartNew(() => this.TransmitData(Token.Parameter));
                    General.Status = Status.CoolDown;
                    this.CoolDownTimer.Restart();
                    this.StartButtonContent = General.CoolDown;
                    break;
                }
                case Status.Run:
                {
                    this.TransmitData(Token.End);
                    break;
                }
                case Status.CoolDown:
                {
                    General.Status = Status.Stop;
                    this.StartButtonContent = General.Stop;
                    this.CoolDownTimer.Restart();
                    break;
                }
                default: break;
            }
        });

        public void ChangeExperimentStatus(int status)
        {
            this.StartButtonContent = status == 0 ? General.Stop : General.Run;
        }

        private void TransmitData(Token token)
        {
            byte id = 1;
            var result = new List<byte> { (byte)token, id };
            if (this.Experiment != null)
            {
                switch (token)
                {
                    case Token.Parameter:
                    {
                        this.Experiment.SetStartTime();
                        byte[] data = General.JsonSerialize(this.Experiment);
                        //var aaa = General.JsonSerializeToString(this.Experiment);
                        if (data != null) result.AddRange(data);
                        break;
                    }
                    case Token.CtValue:
                    {
                        break;
                    }
                    case Token.Query: { byte[] data = General.JsonSerialize(General.WorkingData.Query); if (data != null) result.AddRange(data); break; }
                    default: break;
                }
            }
            General.TcpClient.SendMsg(result.ToArray());
        }
    }
}
