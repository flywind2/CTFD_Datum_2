using CTFD.Global;
using CTFD.Global.Common;
using CTFD.Model.Base;
using CTFD.Model.RuntimeData;
using CTFD.ViewModel.Base;
using CTFD.ViewModel.CommandAction;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CTFD.ViewModel.Monitor
{
    public partial class MonitorViewModel : Base.ViewModel, ISample
    {
        public Visibility IsAccountManagementEnabled { get; set; }

        public Visibility IsSystemSetupEnabled { get; set; }

        private string holeName;

        private string detection;

        private SerialPort qrCodeReceiver;

        public BackgroundTimer CoolDownTimer { get; set; }

        private int experimentViewHeight = 637;
        public int ExperimentViewHeight
        {
            get => this.experimentViewHeight;
            set
            {
                this.experimentViewHeight = value;
                this.RaisePropertyChanged(nameof(this.ExperimentViewHeight));
            }
        }

        public Experiment Experiment => General.WorkingData.Configuration.Experiment;

        public int SelectedSampleIndex { get; set; }

        public string HoleName
        {
            get => this.holeName;
            set
            {
                this.holeName = value;
                foreach (var item in this.Experiment.Samples.Where(o => o.IsSelected == true)) ((Sample)item).SetHoleName(value);
            }
        }

        public string Detection
        {
            get => this.detection;
            set
            {
                this.detection = value;
                foreach (var item in this.Experiment.Samples.Where(o => o.IsSelected == true)) ((Sample)item).SetDetection(value);
            }
        }

        public int AT
        {
            get => this.Experiment.Parameter.AmplificationTemperature / 10;
            set
            {
                if (int.TryParse(value.ToString(), out int result))
                {
                    if (result > 0 && result < 130)
                    {
                        this.Experiment.Parameter.AmplificationTemperature = result * 10;
                        this.RaisePropertyChanged(nameof(this.AT));
                    }
                }
            }
        }

        public int AD
        {
            get => this.Experiment.Parameter.AmplificationDuration / 60;
            set
            {
                if (int.TryParse(value.ToString(), out int result))
                {
                    if (result > 0 && result < 130)
                    {
                        this.Experiment.Parameter.AmplificationDuration = value * 60;
                        this.RaisePropertyChanged(nameof(this.AD));
                    }
                }
            }
        }

        public int DT
        {
            get => this.Experiment.Parameter.LysisTemperature / 10;
            set
            {
                if (int.TryParse(value.ToString(), out int result))
                {
                    if (result > 0 && result < 130)
                    {
                        this.Experiment.Parameter.LysisTemperature = result * 10;
                        this.RaisePropertyChanged(nameof(this.AT));
                    }
                }
            }
        }

        public int DD
        {
            get => this.Experiment.Parameter.LysisDuration / 60;
            set
            {
                if (int.TryParse(value.ToString(), out int result))
                {
                    if (result > 0 && result < 130)
                    {
                        this.Experiment.Parameter.LysisDuration = value * 60;
                        this.RaisePropertyChanged(nameof(this.DD));
                    }
                }
            }
        }

        public string[] MinorSource { get; set; }

        private object startButtonContent;

        public Sample CurrentSample { get; set; } = new Sample();

        public int StepIndex { get; set; } = 0;

        public bool IsStartView => this.GetViewStatus(0);

        public bool IsBasicSetupView => this.GetViewStatus(1);

        public bool IsSampleSetupView => this.GetViewStatus(2);

        public bool IsExprimentView => this.GetViewStatus(3);

        public bool IsAnalysisView => this.GetViewStatus(4);

        public bool IsReportView => this.GetViewStatus(5);

        private bool GetViewStatus(int index) => this.StepIndex == index ? true : false;

        public object StartButtonContent
        {
            get => this.startButtonContent;
            set
            {
                this.startButtonContent = value;
                this.RaisePropertyChanged(nameof(this.StartButtonContent));
            }
        }

        public bool IsMinorEnabled { get; set; }

        public int MajorSelection { get; set; }

        public int MinorSelection { get; set; }

        public RelayCommand MajorSelectionChanged => new RelayCommand(() =>
        {
            this.IsMinorEnabled = true;
            switch (this.MajorSelection)
            {
                case 0:
                {
                    this.Experiment.ChangeSeriesVisibility(this.Experiment.Samples, true);
                    this.MinorSource = new string[0];
                    this.IsMinorEnabled = false;
                    break;
                }
                case 1:
                {
                    this.MinorSource = this.Experiment.Samples.Select(o => o.Detection).Distinct().ToArray();
                    break;
                }
                case 2:
                {
                    this.MinorSource = this.Experiment.Samples.Select(o => o.Name).Distinct().ToArray();
                    break;
                }
                case 3:
                {
                    this.MinorSource = new string[] { "阴性", "阳性" };
                    break;
                }
                default: break;
            }
            this.RaisePropertyChanged(nameof(this.MinorSource));
            this.RaisePropertyChanged(nameof(this.IsMinorEnabled));
            this.MinorSelection = 0;
        });

        public RelayCommand MinorSelectionChanged => new RelayCommand(() =>
        {
            switch (this.MajorSelection)
            {
                case 0:
                {
                    this.Experiment.ChangeSeriesVisibility(this.Experiment.Samples, true);
                    break;
                }
                case 1:
                {
                    var minorItem = this.MinorSource[this.MinorSelection];
                    this.Experiment.ChangeSeriesVisibility(this.Experiment.Samples.Where(o => o.Detection == minorItem).ToArray(), true);
                    break;
                }
                case 2:
                {
                    var minorItem = string.Empty;
                    if (this.MinorSelection >= 0) minorItem = this.MinorSource[this.MinorSelection];
                    if (minorItem != null)
                    {
                        var id = this.Experiment.Samples.Where(o => o.Name == minorItem).ToArray();
                        this.Experiment.ChangeSeriesVisibility(id, true);
                    }
                    break;
                }
            }
        });

        public RelayCommand<int> SwitchViewExperiment => new RelayCommand<int>((viewIndex) => this.SwitchExperimentView(viewIndex));

        public RelayCommand RaiseSelectedSamples => new RelayCommand(() => General.RaiseGlobalHandler(GlobalEvent.RaiseSelectedSamplesFromTable));

        public RelayCommand OpenTemplate => new RelayCommand(() =>
        {

            //var openFileDialog = new System.Windows.Forms.OpenFileDialog { Filter = "Exp（*.xls）|*.xls", FilterIndex = 1, RestoreDirectory = true };
            //if (openFileDialog.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
            //{
            //    this.Device = new Device();
            //    this.Device.InitializeSamples();
            //    this.Device.InitializeChart("");
            //    this.RaisePropertyChanged(nameof(this.Device));
            //    var names = General.ReadExcel(openFileDialog.FileName, "原始数据").AsEnumerable().Select(r => r["A1荧光强度"]).Distinct().ToArray();
            //    this.Device.Samples[0].TestAddingPoint(names);
            //}



            //if (MessageBox.Show("是否覆盖当前已配置的信息", "提示信息", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            //{
            //    var openFileDialog = new System.Windows.Forms.OpenFileDialog { Filter = "Exp（*.sc）|*.sc", FilterIndex = 1, RestoreDirectory = true };
            //    if (openFileDialog.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
            //    {
            //        using (var streamReader = new StreamReader(openFileDialog.FileName))
            //        {
            //            var data = new StringBuilder();
            //            string line = string.Empty;
            //            while ((line = streamReader.ReadLine()) != null) data.Append(line);
            //            this.Device = General.JsonDeserializeFromString<Device>(data.ToString());
            //        }
            //        this.LoadTemplateData();
            //    }
            //}
        });

        public RelayCommand SaveTemplate => new RelayCommand(() =>
        {

            //for (int i = 0; i < 32; i++)
            //{
            //    this.Device?.Samples[i].AddDissolvingPoint(i * new Random().Next(0, 5));
            //}
            //this.TestChartPerformance();

            //var saveFileDialog = new System.Windows.Forms.SaveFileDialog { Filter = "Exp（*.sc）|*.sc", FilterIndex = 1, RestoreDirectory = true };
            //saveFileDialog.FileName = DateTime.Now.ToString("yyyy_MM_dd");
            //if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    try
            //    {
            //        using (var streamWriter = new StreamWriter(new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
            //        {
            //            streamWriter.WriteLine(General.JsonSerializeToString(General.WorkingData.Device));
            //        }
            //        MessageBox.Show("the Job file has been saved");
            //    }
            //    catch (Exception e)
            //    {
            //        MessageBox.Show(e.Message);
            //    }
            //}
        });

        public RelayCommand OpenFluorescenceChart => new RelayCommand(() =>
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog { Filter = "Csv（*.csv）|*.csv", FilterIndex = 1, RestoreDirectory = true };
            if (openFileDialog.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
            {
                try
                {
                    var result = new List<string>();
                    StreamReader reader = new StreamReader(openFileDialog.FileName);
                    string line;
                    while ((line = reader.ReadLine()) != null) { result.Add(line); }

                    this.SetAllStringToCurveData(result);
                    //this.Experiment.AddAmplificationCurve(this.ReadCav(result));
                    General.ShowToast("打开荧光曲线成功");
                }
                catch { }
            }
        });

        public RelayCommand SaveFluorescenceChart => new RelayCommand(() =>
        {
            var saveFileDialog = new System.Windows.Forms.SaveFileDialog { Filter = "Csv（*.csv）|*.csv", FilterIndex = 1, RestoreDirectory = true };
            saveFileDialog.FileName = DateTime.Now.ToString("yyyy_MM_dd");
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    using (var streamWriter = new StreamWriter(new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                    {
                        streamWriter.WriteLine(this.GetAllCurveDataToString());
                    }
                    General.ShowToast($"保存荧光曲线成功");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        });

        public RelayCommand PrintReport => new RelayCommand(async () =>
        {
            var aa = await Func();
            MessageBox.Show(aa.ToString());
        });

        public RelayCommand TransmitParameter => new RelayCommand(() => this.TransmitData(Token.Parameter));

        //public RelayCommand ShowManualSettingView => new RelayCommand(() =>
        //{
        //    ((Window)Activator.CreateInstance(typeof(CTFD.View.Monitor.ManualSettingView), new object[1] { this.Experiment })).ShowDialog();
        //});

        public RelayCommand TurnPrevious => new RelayCommand(() =>
        {
            this.StepIndex--;
            this.SwitchExperimentView(this.StepIndex);
        });

        public RelayCommand TurnNext => new RelayCommand(() =>
        {
            this.StepIndex++;
            this.SwitchExperimentView(this.StepIndex);
        });

        public RelayCommand LostFocus => new RelayCommand(() =>
        {
            General.WriteSetup();
            this.Experiment.ChangeExperimentViewSeries(0);
        });

        public async Task<int> Func()
        {
            await Task.Delay(3000);
            return 100;
        }

        public MonitorViewModel()
        {
            this.InitializeTcpClient(General.WorkingData.Configuration.CurrentTcpServerIPAddress, General.WorkingData.Configuration.TcpServerPort);
            this.InitializeSerialPort();
            this.Experiment.Initialize();
            this.CoolDownTimer = new BackgroundTimer(new DateTime(1, 1, 1, 0, 0, 10), "ss", -1);
            this.CoolDownTimer.Stopped += CoolDownTimer_Stopped;

            this.AddAccount = new RelayCommand(this.ExecuteAddAccount, this.CanExecuteAddAccount);
            this.DeleteAccount = new RelayCommand(this.ExecuteDeleteAccount, this.CanExecuteDeleteAccount);
        }

        private void CoolDownTimer_Stopped(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                this.TransmitData(Token.Start);
            });
        }

        private void InitializeSerialPort()
        {
            var portName = string.Empty;
            try { portName = SerialPort.GetPortNames().First(); }
            catch { }
            if (string.IsNullOrEmpty(portName)) General.ShowToast("扫码枪异常");
            else
            {
                this.qrCodeReceiver = new SerialPort { Encoding = Encoding.Default, PortName = portName };
                this.qrCodeReceiver.DataReceived += this.QrCodeReceiverDataReceived;
                this.qrCodeReceiver.Open();
            }
        }

        private void SwitchExperimentView(int viewIndex)
        {
            if (viewIndex > 10)
            {
                viewIndex = viewIndex % 10;
                this.ExperimentViewHeight = 637;
            }
            else
            {
                if (viewIndex < 0) viewIndex = 0;
                else if (viewIndex > 5) viewIndex = 5;
                this.ExperimentViewHeight = viewIndex > 0 ? 580 : 637;
            }
            this.StepIndex = viewIndex;
            this.RaisePropertyChanged(nameof(this.StepIndex));
            this.RaisePropertyChanged(nameof(this.IsStartView));
            this.RaisePropertyChanged(nameof(this.IsBasicSetupView));
            this.RaisePropertyChanged(nameof(this.IsSampleSetupView));
            this.RaisePropertyChanged(nameof(this.IsExprimentView));
            this.RaisePropertyChanged(nameof(this.IsAnalysisView));
            this.RaisePropertyChanged(nameof(this.IsReportView));
            switch (viewIndex)
            {
                case 0:
                {
                    this.Experiment.Feedbacks[4].Value = General.WorkingData.Configuration.Account.UserName;
                    this.Experiment.User = General.WorkingData.Configuration.Account.UserName;
                    this.Experiment.Feedbacks[3].Value = this.Experiment.ProjectName;
                    break;
                }
                case 1:
                {
                    break;
                }
                case 2:
                {

                    break;
                }
                case 3:
                {
                    if (this.Experiment.IsStarted == false) this.Experiment.RaiseRealtimeXAxis();
                    break;
                }
                case 4:
                {

                    break;
                }
            }
        }

        public void DisposeSerialPort()
        {
            try
            {
                this.qrCodeReceiver.DataReceived -= this.QrCodeReceiverDataReceived;
                this.qrCodeReceiver.DiscardInBuffer();
                this.qrCodeReceiver.DiscardOutBuffer();
                this.qrCodeReceiver.Dispose();
                this.qrCodeReceiver.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadTemplateData()
        {
            var aa = new byte[] { 0x00 };
            var imageSizeBytes = aa.Skip(12).Take(4).ToArray();
            var jobSizeBytes = aa.Skip(16).Take(4).ToArray();

            int imageSize = System.BitConverter.ToInt32(imageSizeBytes, 0);
            int jobSize = System.BitConverter.ToInt32(jobSizeBytes, 0);

            var jobStartIndex = 31 + imageSize;
            var jbo = aa.Skip(jobStartIndex).Take(jobSize).ToArray();
        }

        //public static IList<Object[]> ReadDataFromCSV(string filePathName)
        //{
        //    List<Object[]> ls = new List<Object[]>();
        //    StreamReader fileReader = new StreamReader(filePathName, Encoding.Default);
        //    string line = "";
        //    while (line != null)
        //    {
        //        line = fileReader.ReadLine();
        //        if (String.IsNullOrEmpty(line))
        //            continue;
        //        String[] array = line.Split(';');
        //        ls.Add(array);
        //    }
        //    fileReader.Close();
        //    return ls;
        //}

        private void InitializeTcpClient(string serverIPAddress, int serverPort)
        {
            if (General.TcpClient != null)
            {
                General.TcpClient.ReceiveHandle -= this.DataReceived;
                General.TcpClient.Close();
            }
            General.TcpClient = new Communication.SocketClient(serverIPAddress, serverPort, true);
            General.TcpClient.ReceiveHandle += this.DataReceived;
            General.TcpClient.StartConnecte();
        }

        private void SwitchStatus(Token token, byte data)
        {
            if (data == 0x00)
            {
                if (token == Token.Start)
                {
                    this.StartButtonContent = General.Run;
                    General.Status = Status.Run;
                    General.ShowToast("实验开始");
                    this.Experiment.Start();
                }
                else
                {
                    this.StartButtonContent = General.Stop;
                    General.Status = Status.Stop;
                    General.ShowToast("实验停止");
                    General.WriteSetup();
                    this.Experiment.Stop();
                }
            }
        }

        private void EditCurreentSample(Sample sample, bool isFromBarcode)
        {
            var detections = this.Experiment.Samples.Select(o => o.Detection).Distinct().ToArray();
            if (this.SelectedSampleIndex == -1) this.SelectedSampleIndex = 0;
            var start = this.SelectedSampleIndex / detections.Length * detections.Length;
            var range = start + detections.Length;
            for (int i = start; i < range; i++)
            {
                this.Experiment.Samples[i].Name = sample.Name;
                this.Experiment.Samples[i].Patient.Name = sample.Patient.Name;
                this.Experiment.Samples[i].Patient.Sex = sample.Patient.Sex;
                this.Experiment.Samples[i].Patient.Age = sample.Patient.Age;
                this.Experiment.Samples[i].Patient.CaseId = sample.Patient.CaseId;
                this.Experiment.Samples[i].Patient.BedId = sample.Patient.BedId;
                this.Experiment.Samples[i].Patient.OutPatientId = sample.Patient.OutPatientId;
            }
            if (isFromBarcode) this.SelectedSampleIndex = range;
            else this.SelectedSampleIndex = start;
            if (this.SelectedSampleIndex > 31) this.SelectedSampleIndex = 0;
            this.RaisePropertyChanged(nameof(this.SelectedSampleIndex));
        }

        private void QrCodeReceiverDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var codeString = this.qrCodeReceiver.ReadExisting().TrimEnd();
            if (long.TryParse(codeString, out long barCode))
            {
                this.EditCurreentSample(new Sample(), true);
            }
            else
            {
                //var project = default(Project);
                //try
                //{
                //    project = General.JsonDeserializeFromString<Project>(codeString);
                //    App.Current.Dispatcher.InvokeAsync(() => this.Experiment.ResetParameter(project));
                //    General.ShowToast("√ 扫码实验信息成功");
                //}
                //catch
                //{
                //    General.ShowToast("× 扫码失败请尝试重新扫码");
                //}
            }
        }

        private void DataReceived(Communication.SocketClient client, byte[] message)
        {
            App.Current.Dispatcher.InvokeAsync(() =>
            {
                var token = (Token)message[0];
                var id = message[1];
                var value = message.Skip(2).ToArray();
                switch (token)
                {
                    case Token.Start:
                    case Token.End: { this.SwitchStatus(token, value[0]); break; }
                    case Token.RealtimeAmplificationCurve:
                    {
                        int[] curveData = General.JsonDeserialize<int[]>(value);
                        if (curveData != null) this.Experiment.AddRealtimeAmplificationValue(curveData);
                        break;
                    }
                    case Token.UpperTemperature:
                    case Token.InnerRingTemperature:
                    case Token.OuterRingTemperature:
                    {
                        var data = BitConverter.ToInt16(value, 0);
                        this.Experiment.SetTemperature(token, data);
                        this.Experiment.AddTemperatureValue(token, data);
                        //General.Log.Info($"{token}--{data}");
                        break;
                    }
                    case Token.Rpm: { this.Experiment.SetRpm(BitConverter.ToInt16(value, 0)); break; }
                    case Token.Step: { this.Experiment.SetStep(BitConverter.ToInt16(value, 0)); break; }
                    case Token.Query: { General.RaiseGlobalHandler(GlobalEvent.Query, value); break; }
                    case Token.QueryChart: { General.RaiseGlobalHandler(GlobalEvent.QueryChart, value); break; }
                    case Token.HistoryCurve1: { General.RaiseGlobalHandler(GlobalEvent.HistoryCurve1, value); break; }
                    case Token.HistoryCurve2: { General.RaiseGlobalHandler(GlobalEvent.HistoryCurve2, value); break; }
                    case Token.HistoryCurve3: { General.RaiseGlobalHandler(GlobalEvent.HistoryCurve3, value); break; }
                    case Token.FinalAmplificationCurve:
                    {
                        var curveData = General.JsonDeserialize<List<int[]>>(value);
                        if (curveData != null)
                        {
                            this.Experiment.AddAmplificationCurve(curveData);
                            this.Experiment.SetSectionEnabled(true);
                            this.OnViewChanged();
                        }
                        break;
                    }
                    case Token.RealtimeMeltingCurve:
                    {
                        int[] curveData = General.JsonDeserialize<int[]>(value);
                        if (curveData != null) this.Experiment.AddRealtimeMeltingValue(curveData);
                        break;
                    }
                    case Token.DerivationMeltingCurve:
                    {
                        var curveData = General.JsonDeserialize<List<int[]>>(value);
                        if (curveData != null) this.Experiment.AddDerivationMeltingCurves(curveData);
                        this.Experiment.AddStandardMeltingCurve();
                        break;
                    }
                    case Token.CtValue:
                    {
                        var ctResult = General.JsonDeserialize<string[]>(value);
                        for (int i = 0; i < this.Experiment.Samples.Length; i++) this.Experiment.Samples[i].CtResult = ctResult[i];
                        break;
                    }
                    case Token.TmValue:
                    {
                        var tmResult = General.JsonDeserialize<string[]>(value);
                        for (int i = 0; i < this.Experiment.Samples.Length; i++) this.Experiment.Samples[i].TmResult = tmResult[i];
                        break;
                    }
                    case Token.Fault:
                    {

                        General.ShowFault(true);
                        break;
                    }
                    default: break;
                }
            });
        }

        private string GetAmplificationCurveDataToString(List<int[]> data)
        {
            var result = new StringBuilder();
            var firstRow = new StringBuilder();
            firstRow.Append("时间,");
            var timeStamp = new DateTime(1, 1, 1, 0, 0, 0);
            for (int i = 0; i < data[0].Length; i++)
            {
                timeStamp = timeStamp.AddSeconds(30);
                firstRow.AppendFormat($"{timeStamp.ToString("HH:mm:ss")},");
            }
            result.AppendLine(firstRow.ToString());
            for (int i = 0; i < data.Count; i++)
            {
                var row = new StringBuilder();
                row.Append($"{this.Experiment.Samples[i].HoleName},");
                foreach (var item in data[i]) row.AppendFormat($"{item},");
                result.AppendLine(row.ToString());
            }
            return result.ToString();
        }

        private string GetMeltingCurveDataToString(List<int[]> data)
        {
            var result = new StringBuilder();
            var firstRow = new StringBuilder();
            firstRow.Append("温度,");
            var temp = this.Experiment.Parameter.AmplificationTemperature / 10;
            for (int i = 0; i < data[0].Length; i++) firstRow.AppendFormat($"{ temp + (i * 2)}℃,");

            result.AppendLine(firstRow.ToString());
            for (int i = 0; i < data.Count; i++)
            {
                var row = new StringBuilder();
                row.Append($"{this.Experiment.Samples[i].HoleName},");
                foreach (var item in data[i]) row.AppendFormat($"{item},");
                result.AppendLine(row.ToString());
            }
            return result.ToString();
        }

        private string GetCtValue()
        {
            var result = new StringBuilder();
            var firstRow = new StringBuilder();
            var secondRow = new StringBuilder();
            foreach (var item in this.Experiment.Samples)
            {
                firstRow.AppendFormat($"{item.HoleName},");
                secondRow.AppendFormat($"{item.CtResult ?? string.Empty},");
            }
            result.AppendLine(firstRow.ToString());
            result.AppendLine(secondRow.ToString());
            return result.ToString();
        }

        private string GetAllCurveDataToString()
        {
            var result = new StringBuilder();
            result.AppendLine("扩增曲线");
            result.AppendLine(this.GetAmplificationCurveDataToString(this.Experiment.GetAmplificationData(0).ToList()));
            result.AppendLine("标准熔解曲线");
            result.AppendLine(this.GetMeltingCurveDataToString(this.Experiment.GetAmplificationData(1).ToList()));
            result.AppendLine("导数熔解曲线");
            result.AppendLine(this.GetMeltingCurveDataToString(this.Experiment.GetAmplificationData(2).ToList()));
            result.AppendLine("CT值");
            result.AppendLine(this.GetCtValue());
            return result.ToString();
        }

        private List<int[]> SetStringToCurveData(List<string> data)
        {
            data.RemoveAt(0);
            var result = new List<int[]>(data.Count);
            var values = new List<int>();
            foreach (var item in data)
            {
                foreach (var item2 in item.Split(','))
                {
                    if (int.TryParse(item2, out int value))
                    {
                        values.Add(value);
                    }
                }
                if (values.Count > 0) result.Add(values.ToArray());
                values.Clear();
            }
            return result;
        }

        private void WriteSetupFile()
        {
            var message = "保存设置成功";
            try { General.WriteJsonFile((object)Configuration, $"{Environment.CurrentDirectory}{Properties.Resources.SetupFilePath}", Encoding.Default); }
            catch (Exception exception) { message = exception.Message; }
            General.ShowToast(message);
        }

        private void SetAllStringToCurveData(List<string> data)
        {
            this.Experiment.AddAmplificationCurve(this.SetStringToCurveData(data.Skip(1).Take(33).ToList()));

            this.Experiment.AddStandardMeltingCurve(this.SetStringToCurveData(data.Skip(36).Take(33).ToList()));

            this.Experiment.AddDerivationMeltingCurves(this.SetStringToCurveData(data.Skip(71).Take(33).ToList()));
        }

        public void SetRoleButton()
        {
            this.IsAccountManagementEnabled = Visibility.Visible; this.IsSystemSetupEnabled = Visibility.Visible;
            switch (General.WorkingData.Configuration.Account.Role)
            {
                case "管理员": { this.IsSystemSetupEnabled = Visibility.Collapsed; break; }
                case "操作员": { this.IsAccountManagementEnabled = Visibility.Collapsed; this.IsSystemSetupEnabled = Visibility.Collapsed; break; }
                default: break;
            }
            this.RaisePropertyChanged(nameof(this.IsAccountManagementEnabled));
            this.RaisePropertyChanged(nameof(this.IsSystemSetupEnabled));
            this.SwitchExperimentView(0);
        }

        public void Test()
        {
            //General.ShowFault(true);
            //General.RaiseGlobalHandler(GlobalEvent.Test);
        }

        void ISample.ResetSelection()
        {
            foreach (var item in this.Experiment.Samples) item.IsSelected = false;
        }
    }
}
