using System;
using System.Globalization;
using System.Windows.Data;

namespace CTFD.ViewModel.Converter
{
    public class RowColumnToFocus : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var row = string.Empty;
            switch (values[0])
            {
                case 0: { row = "A"; break; }
                case 1: { row = "B"; break; }
                case 2: { row = "C"; break; }
                case 3: { row = "D"; break; }
                default: break;
            }

            var column = Convert.ToInt32(values[1]) + 1;
            return $"{row}{column}";
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            return null;
        }
    }
}
