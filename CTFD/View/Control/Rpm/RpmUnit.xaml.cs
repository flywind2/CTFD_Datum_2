using CTFD.Global.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace CTFD.View.Control.Rpm
{
    /// <summary>
    /// RpmUnit.xaml 的交互逻辑
    /// </summary>
    public partial class RpmUnit : UserControl
    {

        private SolidColorBrush selectedColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#19526BBF"));
        private SolidColorBrush unSelectedColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#19FFFFFF"));

        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register(nameof(Y2), typeof(double), typeof(RpmUnit), new PropertyMetadata(200D));

        public RpmUnit()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Handle_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            UIElement thumb = e.Source as UIElement;
            this.Y2 += e.VerticalChange;
            if (this.Y2 < 25) this.Y2 = 25;
            else if (this.Y2 > 375) this.Y2 = 375;
            Canvas.SetTop(thumb, this.Y2);
        }

        public void SetSelection(bool isSelected)
        {
            this.Grid1.Background = isSelected ? selectedColor : this.unSelectedColor;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BindingExpression binding = (sender as TextBox).GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }
        }
    }

    public class CanvasTopToRpm : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = Convert.ToDouble(value) - 25;
            var result = Convert.ToInt32(100 - (input / 1.75)) * 40;
            return result;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = Convert.ToInt32(value);
            if (input > 4000) input = 4000;
            if (input < -4000) input = -4000;
            return ((100 - (input / 40)) * 1.75) + 25;

        }
    }
}
