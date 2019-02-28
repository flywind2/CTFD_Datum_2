using CTFD.ViewModel.Base;
using CTFD.ViewModel.Monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace CTFD.View.Monitor
{
    /// <summary>
    /// ManualSettingView.xaml 的交互逻辑
    /// </summary>
    public partial class ManualSettingView : Window
    {
        private CTFD.ViewModel.Base.ViewModel ViewModel;
        public ManualSettingView(object parameter)
        {
            InitializeComponent();
            this.ViewModel = new ManualSettingViewModel(parameter as Model.RuntimeData.Experiment);
            this.DataContext = this.ViewModel;
            this.ViewModel.ViewChanged += ViewModel_ViewChanged;
        }

        private void ViewModel_ViewChanged(object sender, object e)
        {
            this.Close();
        }
    }
}
