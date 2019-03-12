using CTFD.Global.Common;
using CTFD.ViewModel.Monitor;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.Generic;

namespace CTFD.View.Monitor
{
    /// <summary>
    /// MonitorView.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorView : UserControl
    {
        private MonitorViewModel ViewModel;
        public MonitorView()
        {
            InitializeComponent();
            General.Stop = this.Viewbox1.FindResource(nameof(General.Stop));
            General.Run = this.Viewbox1.FindResource(nameof(General.Run));
            General.CoolDown = this.Viewbox1.FindResource(nameof(General.CoolDown));
        }

        private void Instance_Loaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel = this.DataContext as MonitorViewModel;
            General.GlobalHandler += General_GlobalHandler;
        }

        List<Path> p = new List<Path>();
        int b = 0;
        Canvas canvas;
        Line Line1 = new Line { X1 = 0, X2 = 1, Width = 1, Height = 1, Fill = Brushes.Green, Stroke = Brushes.Green, StrokeThickness = 1 };
        private void General_GlobalHandler(object sender, GlobalEventArgs e)
        {
            //switch (e.GlobalEvent)
            //{
            //    case GlobalEvent.Test:
            //    {
            //        b++;
            //        Canvas.SetLeft(this.Line1, b);
            //        var top = Canvas.GetTop(this.Line1);
            //        var sp = new Point(b, top);
            //        var ep = new Point(b + 1, top);
            //        VisualTreeHelper.HitTest(this.canvas, null, f =>
            //        {

            //            if (f.VisualHit is Path path)
            //            {
            //                if (this.p.Contains(path) == false)
            //                {
            //                    this.p.Add(path);
            //                    path.Tag = this.GetXValue(this.canvas.ActualWidth, b, (this.FinalCurve.AxisX[0].ActualMaxValue))/2;

            //                    General.ShowToast($"{path.Tag}");
            //                }
            //            }
            //            return HitTestResultBehavior.Continue;
            //        }, new GeometryHitTestParameters(new RectangleGeometry(new Rect(sp, ep))));
            //        //}, new GeometryHitTestParameters(new RectangleGeometry(new Rect(startPoint, endPoint))));
            //        break;
            //    };
            //    case GlobalEvent.SectionChanged:
            //    {
            //        var aa = (this.FinalCurve.AxisY[0].ActualMaxValue - ((double)e.Value)) / this.FinalCurve.AxisY[0].ActualMaxValue;
            //        General.ShowToast($"{aa}");
            //        break;
            //    }
            //    default: break;
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //this.canvas = (Canvas)this.FinalCurve.Content;

            var aa = this.FinalCurve.Content as Canvas;
            foreach (var item in aa.Children)
            {
                if (item is Canvas canvas)
                {
                    this.canvas = canvas;
                    this.canvas.Background = Brushes.Beige;
                    var index = 0;
                    foreach (var item2 in canvas.Children)
                    {
                        if (item2 is Path path)
                        {
                            path.Name = General.WorkingData.Configuration.Experiment.Samples[index].HoleName;
                            index++;
                        }
                    }
                    this.canvas.Children.Add(this.Line1);
                    Canvas.SetTop(this.Line1, 300);
                    break;
                }
            }
        }

        private void persent_Click(object sender, RoutedEventArgs e)
        {

            General.ShowToast($"当前：{b}--总长{this.canvas.ActualWidth}");
        }

        private double GetCanvasTop(double chartYRange, double threshold, double canvasYRange)
        {
            var percentage = threshold * 100 / chartYRange;
            return canvasYRange - (percentage * canvasYRange / 100);
        }

        private double GetXValue(double canvasXRange,double canvasLeft,double chartXRange)
        {
            return canvasLeft * chartXRange / canvasXRange;
        }

        double startPoint;
        private void FinalCurve_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.p.Clear();
            this.startPoint = this.ViewModel.Experiment.Charts.B;
        }

        private void FinalCurve_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(this.startPoint!=this.ViewModel.Experiment.Charts.B)
            {
                var d = 0;
                var top = this.GetCanvasTop(this.FinalCurve.AxisY[0].ActualMaxValue, this.ViewModel.Experiment.Charts.B, this.canvas.ActualHeight);
                Canvas.SetTop(this.Line1, top);
                b = 0;
                var width = (int)this.canvas.ActualWidth;
                for (int i = 0; i < width; i++)
                {
                    if (p.Count >= 32)
                    {
                        d = i;
                        break;
 
                    }
                    b++;
                    Canvas.SetLeft(this.Line1, b);
                    var sp = new Point(b, top);
                    var ep = new Point(b + 1, top);
                    VisualTreeHelper.HitTest(this.canvas, null, f =>
                    {
                        if (f.VisualHit is Path path)
                        {
                            
                            if (this.p.Contains(path) == false)
                            {
                                this.p.Add(path);
                                path.Tag = this.GetXValue(this.canvas.ActualWidth, b, (this.FinalCurve.AxisX[0].ActualMaxValue)) / 2;
                            }
                        }
                        return HitTestResultBehavior.Continue;
                    }, new GeometryHitTestParameters(new RectangleGeometry(new Rect(sp, ep))));
                }
                var aaa = p;
                var bbb = d;
                //var sb = new System.Text.StringBuilder();
                //foreach (var item in p)
                //{
                //    sb.AppendFormat($"{item.Name}:{((double)item.Tag).ToString("00.00")}-");
                //}
                //General.ShowToast(sb.ToString());
            }
        }
    }
}
