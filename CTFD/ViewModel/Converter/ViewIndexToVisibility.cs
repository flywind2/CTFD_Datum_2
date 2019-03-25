using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CTFD.ViewModel.Converter
{
    public class ViewIndexToVisibility : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var index = Convert.ToInt32(value);
            if (index > 0 && index < 6) return Visibility.Visible;
            else return Visibility.Hidden;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
