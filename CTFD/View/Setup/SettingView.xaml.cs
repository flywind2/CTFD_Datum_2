using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CTFD.View.Setup
{
    /// <summary>
    /// EngineerView.xaml 的交互逻辑
    /// </summary>
    public partial class SettingView : UserControl
    {

        public SettingView()
        {
            InitializeComponent();
           
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
