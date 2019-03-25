using CTFD.Global.Common;
using CTFD.Model.RuntimeData;
using CTFD.View;
using CTFD.View.Login;
using CTFD.ViewModel.Base;
using System;
using System.Windows;

namespace CTFD.ViewModel
{
    public class MainWindowViewModel : Base.ViewModel
    {
        private readonly LoginView loginView;//= new LoginView();
        private readonly WorkingView workingView;// = new WorkingView();
        private WorkingViewModel workingViewModel => this.workingView.DataContext as WorkingViewModel;

        private object contentView;
        public object ContentView
        {
            get { return contentView; }
            set
            {
                contentView = value;
                this.RaisePropertyChanged(nameof(this.ContentView));
            }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; this.RaisePropertyChanged(nameof(this.Title)); }
        }

        public bool IsNotified { get; set; }
        public string Message { get; private set; }

        private Visibility faultStatus = Visibility.Collapsed;
        public Visibility FaultStatus
        {
            get { return this.faultStatus; }
            set
            {
                this.faultStatus = value;
                this.RaisePropertyChanged(nameof(this.FaultStatus));
            }
        }

        private int faultZIndex;
        public int FaultZIndex
        {
            get { return this.faultZIndex; }
            set
            {
                this.faultZIndex = value;
                this.RaisePropertyChanged(nameof(this.FaultZIndex));
            }
        }

        public RelayCommand Test => new RelayCommand(() => this.workingViewModel.MonitorViewModel.Test());

        public RelayCommand Close => new RelayCommand(() =>
        {
            this.workingViewModel.MonitorViewModel.DisposeSerialPort();
        });

        public MainWindowViewModel()
        {
            General.ReadSetup();
            this.loginView = new LoginView();
            this.ContentView = this.loginView;
            General.GlobalHandler += General_GlobalHandler;
            this.InitializeLog();
            //this.InitializeDataBaseOperation();
            this.workingView = new WorkingView();
        }

        private void InitializeLog()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        private void InitializeDataBaseOperation()
        {
            var connectionString = General.ReadConfig(Properties.Resources.ConnectionString);
            General.DataBaseOperation = new Model.Base.DataBaseOperation(connectionString);
        }

        private void General_GlobalHandler(object sender, GlobalEventArgs e)
        {
            switch (e.GlobalEvent)
            {
                case GlobalEvent.Test: { break; }
                case GlobalEvent.ShowLoginView: { this.ShowLoginView(); break; }
                case GlobalEvent.ShowWorkingView: { this.ShowWorkingView(); break; }
                case GlobalEvent.ResetTcpClient: { this.workingViewModel.MonitorViewModel.ResetTcpClient();break; }
                case GlobalEvent.ShowToast: { this.ShowToast(e.Value.ToString()); break; };
                case GlobalEvent.CurveVisibilityChanged: { this.RaiseCurveVisibility(e.Value as Tuple<int, bool>); break; }
                case GlobalEvent.ShowFault: { this.ShowFault((bool)e.Value);break; }
                default: break;
            }
        }

        private void RaiseCurveVisibility(Tuple<int, bool> tuple)
        {
            this.workingViewModel.MonitorViewModel.Experiment.ChangeSeriesVisibility(tuple.Item1, tuple.Item2);
        }

        private void ShowLoginView()
        {
            this.ContentView = this.loginView;
            this.workingViewModel.IsRunningView = true;
        }

        private void ShowWorkingView()
        {
            this.ContentView = this.workingView;
            ((WorkingViewModel)this.workingView.DataContext).MonitorViewModel.SetRoleButton();
        }

        private void ShowMonitorView()
        {
        }

        private void ReadDeviceFromDb()
        {
            var devices = General.DataBaseOperation.Search("select * from tb_device");
            for (int i = 0; i < devices.Rows.Count; i++)
            {
                var name = devices.Rows[i]["DevName"].ToString();

            }
        }

        private void SaveSampleSetting(Sample sample)
        {

        }

        public void ShowToast(string message)
        {
            this.Message = message;
            this.RaisePropertyChanged(nameof(this.Message));
            this.IsNotified = true;
            this.RaisePropertyChanged(nameof(this.IsNotified));
        }

        public void ShowFault(bool isShow)
        {
            this.FaultStatus = isShow ? Visibility.Visible : Visibility.Collapsed;
            this.FaultZIndex = isShow ? 2 : 0;
        }
    }
}
