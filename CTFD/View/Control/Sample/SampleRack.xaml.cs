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

namespace CTFD.View.Control.Sample
{
    /// <summary>
    /// SampleRack.xaml 的交互逻辑
    /// </summary>
    public partial class SampleRack : UserControl
    {
        public SampleRack()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }

    public class NullableToBrush : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return General.WathetColor2;
            else return Brushes.White;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SampleStatusToBrush : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = Convert.ToInt32(value) ^ Convert.ToInt32(parameter);
            return result == 0 ? General.BlueColor : Brushes.White;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
