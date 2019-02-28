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
    /// Toast.xaml 的交互逻辑
    /// </summary>
    public partial class Toast : UserControl
    {
        System.Windows.Media.Animation.Storyboard storyboard;

        public bool IsPopup
        {
            get { return (bool)GetValue(PopupProperty); }
            set { SetValue(PopupProperty, value); }
        }
        public static readonly DependencyProperty PopupProperty =
            DependencyProperty.Register(nameof(IsPopup), typeof(bool), typeof(Toast), new PropertyMetadata(false, Callback_IsPopup));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(Toast), new PropertyMetadata("Message"));

        public Toast()
        {
            InitializeComponent();
        }

        private static void Callback_IsPopup(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (Toast)d;
            if (instance.storyboard == null) instance.storyboard = instance.Resources["Opacity"] as System.Windows.Media.Animation.Storyboard;
            instance.storyboard.Completed += (sender, eventArgs) => Panel.SetZIndex(instance, 0);
            if ((bool)e.NewValue)
            {
                Panel.SetZIndex(instance, 2);
                instance.storyboard.Begin(instance);
            }
            instance.IsPopup = false;
        }
    }
}
