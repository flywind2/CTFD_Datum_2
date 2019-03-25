using Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;

namespace CtfdSimulator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private SocketServer tcpServer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private List<byte> ReadJsonBytes(string path)
        {
            var result = new List<byte>();
            var stringBuilder = new StringBuilder();
            StreamReader sr = new StreamReader(path, Encoding.Default);
            string line;
            while ((line = sr.ReadLine()) != null) stringBuilder.Append(line);
            var jsonString = stringBuilder.ToString();
            for (int i = 0; i < jsonString.Length / 2; i++) result.Add(Convert.ToByte(jsonString.Substring(i * 2, 2), 16));
            return result;
        }

        private void RaiseUpperMessage(string message)
        {
            App.Current.Dispatcher.InvokeAsync(() => this.ListBoxMessage.Items.Add(message));
        }

        private void SendMessage(byte id, byte token, byte[] datas, string message)
        {
            if (this.tcpServer != null)
            {
                var messageBytes = new List<byte> { id, token };
                messageBytes.AddRange(datas);
                this.tcpServer.SendToClient(messageBytes.ToArray());
            }
            else message = "请先启动服务";
            this.RaiseUpperMessage(message);
        }

        private void StartService(object sender, RoutedEventArgs e)
        {
            if (this.tcpServer == null)
            {
                this.tcpServer = new SocketServer(this.TextBoxIPAddress.Text, Convert.ToInt32(this.TextBoxPort.Text));
                this.tcpServer.ReceiveHandle += (client, args) =>
                {
                    switch (args[0])
                    {
                        case 0: { this.RaiseUpperMessage("初始化↙"); break; }
                        case 1: { this.RaiseUpperMessage("参数设置↙"); break; }
                        case 2: { this.RaiseUpperMessage("开始实验↙"); break; }
                        case 3: { this.RaiseUpperMessage("结束实验↙"); break; }
                        case 8: { this.RaiseUpperMessage("查询实验↙"); break; }
                        case 9: { this.RaiseUpperMessage("查询曲线↙"); break; }
                    }
                };
                this.tcpServer.Start();
                this.RaiseUpperMessage("开启服务");
            }
            else this.RaiseUpperMessage("服务已经开启");
        }

        private void StopService(object sender, RoutedEventArgs e)
        {
            if (this.tcpServer != null)
            {
                this.tcpServer.Stop();
                this.tcpServer = null;
                this.RaiseUpperMessage("停止服务");
            }
            else this.RaiseUpperMessage("没有可用的服务");
        }

        private void SendTemperature(object sender, RoutedEventArgs e)
        {
            var data = BitConverter.GetBytes(Convert.ToInt32(Convert.ToSingle(this.TbxTem.Text) * 10)).Take(2).ToArray();
            switch (this.CbxTem.SelectedIndex)
            {
                case 0: { this.SendMessage(5, 0, data, "发送上盖温度给上位机 ↗"); break; }
                case 1: { this.SendMessage(6, 0, data, "发送裂解温度给上位机 ↗"); break; }
                case 2: { this.SendMessage(16, 0, data, "发送扩增温度给上位机 ↗"); break; }
                default: break;
            }
        }

        private void SendRpm(object sender, RoutedEventArgs e)
        {
            this.SendMessage(7, 0, BitConverter.GetBytes(Convert.ToInt32(this.TextBoxRpm.Text)).Take(2).ToArray(), "发送转速信息给上位机 ↗");
        }

        private void SendStep(object sender, RoutedEventArgs e)
        {
            this.SendMessage(8, 0, BitConverter.GetBytes(this.ComboBoxStep.SelectedIndex).Take(2).ToArray(), $"发送实验状态 '{this.ComboBoxStep.SelectedValue.ToString().Split(':')[1]}' 给上位机 ↗");
        }

        private void ClickCleaningList(object sender, RoutedEventArgs e)
        {
            this.ListBoxMessage.Items.Clear();
        }

        private void ClickSendingStartFeedback(object sender, RoutedEventArgs e)
        {
            switch (this.CbxSs.SelectedIndex)
            {
                case 0: { this.SendMessage(2, 0, new byte[] { (byte)this.CbxFb.SelectedIndex }, $"发送启动实验 '{this.CbxFb.SelectedValue.ToString().Split(':')[1]}' 给上位机↗"); break; }
                case 1: { this.SendMessage(3, 0, new byte[] { (byte)this.CbxFb.SelectedIndex }, $"发送停止实验 '{this.CbxFb.SelectedValue.ToString().Split(':')[1]}' 给上位机↗"); break; }
                default: break;
            }
        }

        private void SendChartData(object sender, RoutedEventArgs e)
        {
            var result = default(byte[]);
            byte token = 4;
            if (this.ComboBoxChartType.SelectedIndex > 0) token = (byte)(11 + this.ComboBoxChartType.SelectedIndex);
            switch (this.ComboBoxChartType.SelectedIndex)
            {
                // 实时荧光曲线
                case 0:
                case 2:
                {
                    var chartData = new int[32];
                    for (int i = 0; i < 32; i++) chartData[i] = new Random().Next(5) * i;
                    result = JsonSerialize(chartData);
                    break;
                }
                // 结果曲线
                default:
                {
                    var chartData = new List<int[]>(32);
                    for (int i = 0; i < 32; i++)
                    {
                        chartData.Add(new int[60]);
                        for (int j = 0; j < 60; j++)
                        {
                            chartData[i][j] = new Random().Next(i * i + j) * j;
                        }
                    }
                    result = JsonSerialize(chartData);
                    break;
                }
            }
            this.SendMessage(token, 0, result, $"发送 '{this.ComboBoxChartType.SelectedValue.ToString().Split(':')[1]}' 给上位机 ↗");
        }

        private static byte[] JsonSerialize<T>(T entity)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(entity.GetType());
            var result = default(byte[]);
            using (MemoryStream stream = new MemoryStream())
            {
                jsonSerializer.WriteObject(stream, entity);
                result = stream.ToArray();
            }
            return result;
        }

        private void Query_Click(object sender, RoutedEventArgs e)
        {
            var result = JsonSerialize(this.GetTestQueryData());
            this.SendMessage(0x09, 0, result, "发送实验结果给上位机 ↗");
        }

        private List<Experiment> GetTestQueryData()
        {
            var result = new List<Experiment>(3);

            var parameters = new Parameter[] { new Parameter { LysisTemperature = 96 }, new Parameter { LysisTemperature = 97 }, new Parameter { LysisTemperature = 98 } };
            for (int i = 0; i < 3; i++)
            {
                //var feedBacks = new Feedback[5] { new Feedback(), new Feedback(), new Feedback(), new Feedback(), new Feedback { Value = $"操作员{i}" } };
                var experiment = new Experiment { Name = $"实验{i}", GroupName = $"组{i}", ProjectName = $"项目{i}", User=$"实验员{i}", StartTime = DateTime.Now.ToString("yyyyMMdd-HH:mm"), Parameter = parameters[i] };
                result.Add(experiment);
            }
            return result;
        }
    }

    [DataContract]
    public class Experiment
    {
        [DataMember]
        public string StartTime { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string User { get; set; }

        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public string ProjectName { get; set; }

        [DataMember]
        public Parameter Parameter { get; set; } = new Parameter();

        [DataMember]
        public Feedback[] Feedbacks { get; set; }

        [DataMember]
        public Sample[] Samples { get; set; }
    }

    [DataContract]
    public class Parameter
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
        /// 高转速
        /// </summary>
        [DataMember]
        public int HighSpeed { get; set; } = 4600;

        /// <summary>
        /// 高速时间
        /// </summary>
        [DataMember]
        public int HighSpeedDuration { get; set; } = 30;

        /// <summary>
        /// 熔解时间
        /// </summary>
        [DataMember]
        public int MeltDuration { get; set; } = 30 * 60;

        /// <summary>
        /// 是否熔解
        /// </summary>
        [DataMember]
        public bool IsMelt { get; set; }
    }

    [DataContract]
    public class Feedback
    {
        [DataMember]
        public Visibility Status { get; set; }

        public string Value { get; set; }
    }

    [DataContract]
    public class Sample
    {
        [DataMember]
        public string HoleName { get; private set; }

        [DataMember]
        public string Detection { get; private set; }
    }
}
