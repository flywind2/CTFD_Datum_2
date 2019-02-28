using LiveCharts;
using System.Windows;
using System.Windows.Controls;

namespace CTFD.View.Control.Chart
{
    /// <summary>
    /// DissolvingCurve.xaml 的交互逻辑
    /// </summary>
    public partial class DissolvingCurve : UserControl
    {
        public SeriesCollection SeriesCollection
        {
            get { return (SeriesCollection)GetValue(SeriesCollectionProperty); }
            set { SetValue(SeriesCollectionProperty, value); }
        }
        public static readonly DependencyProperty SeriesCollectionProperty =
            DependencyProperty.Register(nameof(SeriesCollection), typeof(SeriesCollection), typeof(DissolvingCurve), new PropertyMetadata(null));

        public DissolvingCurve()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}

