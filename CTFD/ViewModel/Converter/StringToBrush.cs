using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CTFD.ViewModel.Converter
{
    public class StringToBrush : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = Brushes.Transparent;
            if (value != null)
            {
                var color = value.ToString();
                if (string.IsNullOrEmpty(color) == false) result = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            }
            return result;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = string.Empty;
            if (value != null) result = ((SolidColorBrush)value).ToString();
            return result;
        }
    }
}
