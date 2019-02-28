using CTFD.Global.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CTFD.ViewModel.Behavior
{
    abstract public class MultiSelection : BehaviorBase
    {
        protected bool IsHandled { get; set; }
        protected abstract HitTestResultCallback MouseDownHitTestResultCallback { get; }
        protected abstract HitTestResultCallback MouseMoveHitTestResultCallback { get; }
        protected abstract HitTestResultCallback MouseUpHitTestResultCallback { get; }
        public abstract object ViewModel { get; set; }
        private Point firstPoint;
        protected Rectangle frame;
        protected bool isLoaded = false;
        bool isMouseDown;
        protected Canvas cs;

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(MultiSelection), new PropertyMetadata(Brushes.Transparent));

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(nameof(Stroke), typeof(Brush), typeof(MultiSelection), new PropertyMetadata(Brushes.Transparent));

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseMove += AssociatedObject_MouseMove;
            this.AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
            this.AssociatedObject.PreviewMouseLeftButtonUp += AssociatedObject_PreviewMouseLeftButtonUp;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
            this.AssociatedObject.PreviewMouseMove -= AssociatedObject_MouseMove;
            this.AssociatedObject.PreviewMouseLeftButtonUp -= AssociatedObject_PreviewMouseLeftButtonUp;
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = (Canvas)sender;
            this.firstPoint = e.GetPosition((IInputElement)sender);
            for (int i = 0; i < canvas.Children.Count; i++) if (canvas.Children[i] is Rectangle r) canvas.Children.Remove(r);
            VisualCollection visualHost = new VisualCollection(canvas);
            frame = new Rectangle();
            this.isMouseDown = true;
            Canvas.SetLeft(frame, this.firstPoint.X);
            Canvas.SetTop(frame, this.firstPoint.Y);
            frame.Width = 0;
            frame.Height = 0;
            frame.Fill = this.Fill;
            frame.Stroke = this.Stroke;
            frame.StrokeDashArray = new DoubleCollection() { 2 };
            frame.StrokeThickness = 3;
            canvas.Children.Add(this.frame);

            VisualTreeHelper.HitTest
           (
               this.AssociatedObject,
               null,
               this.MouseDownHitTestResultCallback,
               new GeometryHitTestParameters(new EllipseGeometry(this.firstPoint, 2, 2))
           );
            e.Handled = this.IsHandled;
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.isMouseDown)
            {
                e.Handled = this.IsHandled;
                this.cs = sender as Canvas;

                //cs.Dispatcher.BeginInvoke(new Action(() => { System.Windows.Forms.SendKeys.Send("^"); }), System.Windows.Threading.DispatcherPriority.Input);
                //if (Keyboard.IsKeyDown(Key.LeftCtrl)) viewModel.Test = General.BlueColor;
                //else viewModel.Test = General.ChartColor1;
                var oldPoint = e.GetPosition((IInputElement)sender);
                var width = oldPoint.X - this.firstPoint.X;
                var height = oldPoint.Y - this.firstPoint.Y;
                var startPoint = this.firstPoint;
                var endPoint = oldPoint;
                if (width > 0)
                {
                    isLoaded = true;
                    if (height < 0)
                    {
                        startPoint = new Point(this.firstPoint.X, oldPoint.Y);
                        endPoint = new Point(oldPoint.X, this.firstPoint.Y);
                        Canvas.SetTop(frame, oldPoint.Y);
                    }
                }
                else
                {
                    isLoaded = false;
                    if (height > 0)
                    {
                        startPoint = new Point(oldPoint.X, this.firstPoint.Y);
                        endPoint = new Point(this.firstPoint.X, oldPoint.Y);
                        Canvas.SetLeft(frame, oldPoint.X);
                    }
                    else
                    {
                        startPoint = new Point(oldPoint.X, oldPoint.Y);
                        endPoint = new Point(this.firstPoint.X, this.firstPoint.Y);
                        Canvas.SetLeft(frame, oldPoint.X);
                        Canvas.SetTop(frame, oldPoint.Y);
                    }
                }

                frame.Width = Math.Abs(width);
                frame.Height = Math.Abs(height);
                VisualTreeHelper.HitTest
                (
                    this.AssociatedObject,
                    null,
                    this.MouseMoveHitTestResultCallback,
                    new GeometryHitTestParameters(new RectangleGeometry(new Rect(startPoint, endPoint)))
                );
            }
        }

        private void AssociatedObject_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isMouseDown = false;
            var canvas = ((Canvas)sender);
            canvas.Children.Remove(this.frame);
            var secondPoint = e.GetPosition((IInputElement)sender);
            var deltaX = Math.Abs(this.firstPoint.X - secondPoint.X);
            var deltaY = Math.Abs(this.firstPoint.Y - secondPoint.Y);
            if (deltaX > 5 || deltaY > 5) e.Handled = true;

            if (deltaX < 5 && deltaY < 5)
            {
                VisualTreeHelper.HitTest
                (
                    this.AssociatedObject,
                    null,
                    this.MouseUpHitTestResultCallback,
                    new GeometryHitTestParameters(new EllipseGeometry(secondPoint, 2, 2))
                );
            }

            General.RaiseGlobalHandler(GlobalEvent.RaiseSelectedSamplesFromRack);
        }
    }
}
