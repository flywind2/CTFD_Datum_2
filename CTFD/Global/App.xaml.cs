using System;
using System.Windows;

namespace CTFD.Global
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.StartupUri = new Uri("/CTFD;component/View/MainWindow.xaml", UriKind.Relative);
        }
    }
}
