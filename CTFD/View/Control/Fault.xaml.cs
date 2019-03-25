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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CTFD.View.Control
{
    /// <summary>
    /// Fault.xaml 的交互逻辑
    /// </summary>
    public partial class Fault : UserControl
    {
        System.Windows.Media.Animation.Storyboard storyboard;

        //public bool IsPopup
        //{
        //    get { return (bool)GetValue(PopupProperty); }
        //    set { SetValue(PopupProperty, value); }
        //}
        //public static readonly DependencyProperty PopupProperty =
        //    DependencyProperty.Register(nameof(IsPopup), typeof(bool), typeof(Fault), new PropertyMetadata(false, Callback_IsPopup));

        public Fault()
        {
            InitializeComponent();
        }

        //private static void Callback_IsPopup(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var instance = (Fault)d;
        //    if (instance.storyboard == null) instance.storyboard = instance.Resources["Opacity"] as System.Windows.Media.Animation.Storyboard;
        //    if ((bool)e.NewValue)
        //    {
        //        Panel.SetZIndex(instance, 2);
        //        instance.storyboard.Begin(instance);
        //    }
        //    else Panel.SetZIndex(instance, 0);
        //}
    }
}
