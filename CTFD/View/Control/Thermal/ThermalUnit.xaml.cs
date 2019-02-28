using CTFD.Global.Common;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace CTFD.View.Control.Thermal
{
    /// <summary>
    /// ThermalRegion.xaml 的交互逻辑
    /// </summary>
    public partial class ThermalUnit : UserControl
    {
        public int test
        {
            get { return (int)GetValue(testProperty); }
            set { SetValue(testProperty, value); }
        }
        public static readonly DependencyProperty testProperty =
            DependencyProperty.Register("test", typeof(int), typeof(ThermalUnit), new PropertyMetadata(0));



        public bool IsSelected { get;private set; }

        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register(nameof(Y1), typeof(double), typeof(ThermalUnit), new PropertyMetadata(300D));

        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register(nameof(Y2), typeof(double), typeof(ThermalUnit), new PropertyMetadata(300D));

        public Brush PlateauColor
        {
            get { return (Brush)GetValue(PlateauColorProperty); }
            set { SetValue(PlateauColorProperty, value); }
        }
        public static readonly DependencyProperty PlateauColorProperty =
            DependencyProperty.Register(nameof(PlateauColor), typeof(Brush), typeof(ThermalUnit), new PropertyMetadata(General.BlueColor));

        public ThermalUnit()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            UIElement thumb = e.Source as UIElement;
            this.Y2 += e.VerticalChange;
            if (this.Y2 > 300) this.Y2 = 300;
            else if (this.Y2 < 4) this.Y2 = 4;
            Canvas.SetTop(thumb, this.Y2);
        }

        public void SetSelection(bool isSelected)
        {
            this.IsSelected = isSelected;
            this.PlateauColor = isSelected ? General.GreenColor : General.BlueColor;
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

    public class CanvasTopToTemperature : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (float.TryParse(value.ToString(), out float input)) return (100 - (Convert.ToInt32(input) / 4));
            else return 25;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (float.TryParse(value.ToString(), out float input))
            {
                if (input < 25) input = 25;
                else if (input > 99) input = 99;
                return ((100 - Convert.ToInt32(input)) * 4);
            }
            else return 300;
        }
    }
}
