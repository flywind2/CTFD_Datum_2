using CTFD.Global.Common;
using CTFD.ViewModel.Monitor;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;

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
            General.GlobalHandler += General_GlobalHandler;
        }

        private void General_GlobalHandler(object sender, GlobalEventArgs e)
        {
            switch (e.GlobalEvent)
            {
                case GlobalEvent.SectionChanged:
                {

                    //var canvas = this.FinalCurve;
                    //var bb = false;
                    //VisualTreeHelper.HitTest(this.Viewbox1, null, f =>
                    //{
                    //    var aaa = f.VisualHit;
                    //    //General.ShowToast();
                    //    return HitTestResultBehavior.Continue;
                    //}, new GeometryHitTestParameters(this.Section.);

                    //General.ShowToast(mouse);
                    break;
                };
                default:break;
            }
        }
    }
}
