using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CTFD.ViewModel.Converter
{
    public class BoolInversion : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !Convert.ToBoolean(value);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !Convert.ToBoolean(value);
        }
    }   

    public class VisibilityToBool : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible ? true : false;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert.ToBoolean(value) ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    //public class RoleToInt32 : IValueConverter
    //{
    //    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return Convert.ToInt32(value);
    //    }

    //    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return (Role)value;
    //    }
    //}

    //public class RoleToString : IValueConverter
    //{
    //    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        var result = string.Empty;
    //        switch (Convert.ToInt32(value))
    //        {
    //            case 0: { result = "工程师"; break; }
    //            case 1: { result = "管理员"; break; }
    //            case 2: { result = "操作员"; break; }
    //            default: break;
    //        }
    //        return result;
    //    }

    //    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return (Role)value;
    //    }
    //}
}
