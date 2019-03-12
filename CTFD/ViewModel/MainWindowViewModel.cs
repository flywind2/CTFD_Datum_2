using CTFD.Global;
using CTFD.Global.Common;
using CTFD.Model.Base;
using CTFD.Model.RuntimeData;
using CTFD.View;
using CTFD.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
            this.InitializeDataBaseOperation();
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
                case GlobalEvent.ShowToast: { this.ShowToast(e.Value.ToString()); break; };
                case GlobalEvent.CurveVisibilityChanged: { this.RaiseCurveVisibility(e.Value as Tuple<int, bool>); break; }
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
    }
}
