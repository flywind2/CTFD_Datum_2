using CTFD.Global.Common;
using CTFD.Model.RuntimeData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Hole.xaml 的交互逻辑
    /// </summary>
    public partial class Hole : UserControl
    {
        private DoubleCollection doubleCollection = new DoubleCollection { 2, 2 };
        public Hole()
        {
            InitializeComponent();
        }

     
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(Hole), new PropertyMetadata(false, new PropertyChangedCallback((d, e) =>
            {
                if (e.NewValue != null)
                {
                    var hole = d as Hole;
                    var isSelected = (bool)e.NewValue;
                    if (isSelected)
                    {
                        hole.Ellipse1.StrokeDashArray = hole.doubleCollection;
                        hole.Ellipse1.Fill = General.WathetColor2;
                    }
                    else
                    {
                        hole.Ellipse1.StrokeDashArray = null;//hole.doubleCollection;
                        hole.Ellipse1.Fill = Brushes.White;
                    }
                }
            })));


        public bool IsLoaded
        {
            get { return (bool)GetValue(IsLoadedProperty); }
            set { SetValue(IsLoadedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLoaded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadedProperty =
            DependencyProperty.Register("IsLoaded", typeof(bool), typeof(Hole), new PropertyMetadata(false, new PropertyChangedCallback((d, e) =>
            {
                if (e.NewValue != null)
                {
                    var hole = d as Hole;
                    var isLoaded = (bool)e.NewValue;
                    if (isLoaded)
                    {
                        hole.Ellipse1.Stroke = General.BlueColor;
                    }
                    else
                    {
                        hole.Ellipse1.Stroke = General.GrayColor;
                    }
                }
            })));

    }

    public class NullableToVisibility : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;
            else return Visibility.Visible;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
