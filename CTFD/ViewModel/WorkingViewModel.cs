using CTFD.Global.Common;
using CTFD.View;
using CTFD.View.Help;
using CTFD.View.History;
using CTFD.View.Monitor;
using CTFD.View.Setup;
using CTFD.ViewModel.Monitor;
using System.Linq;

namespace CTFD.ViewModel
{
    public class WorkingViewModel : Base.ViewModel
    {
        private ViewState viewState = ViewState.RunningView;

        private readonly MonitorView monitorView = new MonitorView { DataContext = new MonitorViewModel() };
        
        private readonly HelpView helpView = new HelpView();

        private readonly SettingView setupView = new SettingView();

        private readonly HistoryView historyView = new HistoryView();

        public MonitorViewModel MonitorViewModel => this.monitorView.DataContext as MonitorViewModel;

        public bool IsRunningView
        {
            get => this.viewState == ViewState.RunningView ? true : false;
            set
            {
                if (value)
                {
                    this.ContentView = this.monitorView;
                    this.viewState = ViewState.RunningView;
                    this.RaiseViewState();
                }
            }
        }

        public bool IsHelpView
        {
            get => this.viewState == ViewState.HelpView ? true : false;
            set
            {
                if (value)
                {
                    this.ContentView = this.helpView;
                    this.viewState = ViewState.HelpView;
                    this.RaiseViewState();
                }
            }
        }

        public bool IsLoginView
        {
            get => false;
            set
            {
                General.RaiseGlobalHandler(GlobalEvent.ShowLoginView);
            }
        }

        public bool IsHistoryView
        {
            get => this.viewState == ViewState.HistoryView ? true : false;
            set
            {
                if (value)
                {
                    this.ContentView = this.historyView;
                    this.viewState = ViewState.HistoryView;
                    this.RaiseViewState();
                }
            }
        }

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

        public WorkingViewModel()
        {
            this.ContentView = this.monitorView;
            this.MonitorViewModel.StartButtonContent = General.Stop;
        }

        private void RaiseViewState()
        {
            this.RaisePropertyChanged(nameof(this.IsRunningView));
            this.RaisePropertyChanged(nameof(this.IsHelpView));
            this.RaisePropertyChanged(nameof(this.IsLoginView));
            this.RaisePropertyChanged(nameof(this.IsHistoryView));
        }
    }
}
