using CTFD.Global.Common;
using CTFD.ViewModel.Monitor;
using System.Windows;
using System.Windows.Controls;

namespace CTFD.View.Monitor
{
    /// <summary>
    /// MonitorView.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorView : UserControl
    {
       

        private MonitorViewModel ViewModel;
        public MonitorView()
        {
            InitializeComponent();
            General.Stop = this.Viewbox1.FindResource(nameof(General.Stop));
            General.Run = this.Viewbox1.FindResource(nameof(General.Run));
            General.CoolDown = this.Viewbox1.FindResource(nameof(General.CoolDown));
        }

        private void Instance_Loaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel = this.DataContext as MonitorViewModel;

        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            UIElement thumb = e.Source as UIElement;
            var aa = this.ViewModel.Margin.Top+e.VerticalChange;
            this.ViewModel.Margin = new Thickness(0, aa, 0, 0);
        }

        private void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
