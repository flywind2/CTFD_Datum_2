using CTFD.Global.Common;
using CTFD.ViewModel.Monitor;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;

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
            this.ViewModel.ViewChanged += ViewModel_ViewChanged;
        }

        private void ViewModel_ViewChanged(object sender, object e) => Application.Current.Dispatcher.InvokeAsync(() => this.ViewModel.Experiment.Charts.IsDisplaySection = true);

        int increment = 0;
        Canvas canvas;
        Line cueObject = new Line { X1 = 0, X2 = 1, Width = 1, Height = 1, Fill = Brushes.Green, Stroke = Brushes.Green, StrokeThickness = 1 };

        private void InitializeHitTest()
        {
            //General.Log.Info($"0 初始化CT值计算.............................................");
            
            var xIndex = 0;
            var yIndex = 0;
            
            foreach (var item in (this.FinalCurve.Content as Canvas).Children)
            {
                //General.Log.Info($"1 寻找内容元素");
                if (item is Canvas canvas)
                {
                    //General.Log.Info($"1.1 找到画布Canvas");
                    this.canvas = canvas;
                    var index = 0;

                    foreach (var item2 in canvas.Children)
                    {
                        //General.Log.Info($"1.1.1 寻找曲线元素Path");
                        if (item2 is Path path)
                        {
                            path.Name = General.WorkingData.Configuration.Experiment.Samples[index].HoleName;
                            index++;
                            //General.Log.Info($"1.1.1.1 找到第：{index}条曲线 -- {path.Name}");
                        }
                    }
                    if (this.canvas.Children.Contains(this.cueObject) == false) this.canvas.Children.Add(this.cueObject);
                }
                else if (item is Line line)
                {
                    //General.Log.Info($"1.2 找到网格线");
                    if (line.ActualWidth > 100)
                    {
                        if (xIndex == 0)
                        {
                            line.StrokeThickness = 2;
                            line.Stroke = General.BlueColor;
                        }
                        xIndex++;
                    }
                    else if (line.ActualHeight > 100)
                    {
                        if (yIndex == 0)
                        {
                            line.StrokeThickness = 2;
                            line.Stroke = General.BlueColor;
                        }
                        yIndex++;
                    }
                }
            }
        }

        private List<Path> CalculateCtValue()
        {
            var pathCache = new List<Path>();
            var top = this.GetCanvasTop(this.FinalCurve.AxisY[0].ActualMaxValue, this.ViewModel.Experiment.Charts.Threshold, this.canvas.ActualHeight);
            Canvas.SetTop(this.cueObject, top);
            increment = 0;
            var width = (int)this.canvas.ActualWidth;
            for (int i = 0; i < width; i++)
            {
                if (pathCache.Count >= 32) break;
                increment++;
                Canvas.SetLeft(this.cueObject, increment);
                var startPoint = new Point(increment, top);
                var endPoint = new Point(increment + 1, top);
                VisualTreeHelper.HitTest(this.canvas, null, f =>
                {
                    if (f.VisualHit is Path path)
                    {
                        if (pathCache.Contains(path) == false)
                        {
                            pathCache.Add(path);
                            path.Tag = this.GetXValue(this.canvas.ActualWidth, increment, (this.FinalCurve.AxisX[0].ActualMaxValue)) / 2;
                        }
                    }
                    return HitTestResultBehavior.Continue;
                }, new GeometryHitTestParameters(new RectangleGeometry(new Rect(startPoint, endPoint))));
            }
            return pathCache;
        }

        private double GetCanvasTop(double chartYRange, double threshold, double canvasYRange)
        {
            var percentage = threshold * 100 / chartYRange;
            return canvasYRange - (percentage * canvasYRange / 100);
        }

        private double GetXValue(double canvasXRange, double canvasLeft, double chartXRange)
        {
            return canvasLeft * chartXRange / canvasXRange;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.InitializeHitTest();

                var samples = this.ViewModel.Experiment.Samples;
                //General.Log.Info($"2 准备计算CT值--设阈值：{this.ViewModel.Experiment.Charts.Threshold}");
                for (int i = 0; i < this.ViewModel.Experiment.Samples.Length; i++) samples[i].CtResult = string.Empty;
                //General.Log.Info($"3 清空CT值 当前样本个数：{this.ViewModel.Experiment.Samples.Length}");

                var ctValue = this.CalculateCtValue();
                //General.Log.Info($"4 CT值计算完毕 CT值个数：{ctValue.Count}");
                foreach (var item in ctValue)
                {
                    var sample = samples.FirstOrDefault(o => o.HoleName == item.Name);
                    if (sample != null)
                    {
                        sample.CtResult = ((double)item.Tag).ToString("0.00");
                        //General.Log.Info($"5 找到样本：{sample.HoleName} -- CT值：{ sample.CtResult}");
                    }
                    //else General.Log.Info($"5 没有找到对应的样本：{item.Name}");
                }
                //General.Log.Info("6 完成CT值计算............................................................");
                this.ViewModel.TransmitCtValue();
            }
            catch (System.Exception ex)
            {
                General.Log.Error(ex.Message);
            }
        }
    }
}
