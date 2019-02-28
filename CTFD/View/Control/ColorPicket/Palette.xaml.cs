using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace CTFD.View.Control.ColorPicket
{
    /// <summary>
    /// Palette.xaml 的交互逻辑
    /// </summary>
    public partial class Palette : UserControl
    {
        private double leftThumbIncrementX;
        private double leftThumbIncrementY;

        private System.Drawing.Bitmap RightPalette;
        private System.Drawing.Bitmap LeftPalette;

        public Brush BasicColor
        {
            get { return (Brush)GetValue(BasicColorProperty); }
            set { SetValue(BasicColorProperty, value); }
        }
        public static readonly DependencyProperty BasicColorProperty =
            DependencyProperty.Register(nameof(BasicColor), typeof(Brush), typeof(Palette), new PropertyMetadata(Brushes.Red));

        public Brush SelectedColor
        {
            get { return (Brush)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(nameof(SelectedColor), typeof(Brush), typeof(Palette), new PropertyMetadata(Brushes.Black));

        public Palette()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            this.RightPalette = ConvertBitmapSourceToBitmap(GetTargetBitmap(this.Canvas_RightPalette));
            this.LeftPalette = ConvertBitmapSourceToBitmap(GetTargetBitmap(this.Canvas_LeftPalette));
        }

        private RenderTargetBitmap GetTargetBitmap(FrameworkElement targetVisual)
        {
            var result = new RenderTargetBitmap((int)targetVisual.ActualWidth, (int)targetVisual.ActualHeight, 96, 96, PixelFormats.Default);
            result.Render(targetVisual);
            return result;
        }

        private System.Drawing.Bitmap ConvertBitmapSourceToBitmap(BitmapSource bitmapSource)
        {
            var result = default(System.Drawing.Bitmap);
            using (System.IO.MemoryStream memorStream = new System.IO.MemoryStream())
            {
                var bmpBitmapEncoder = new BmpBitmapEncoder();
                bmpBitmapEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)bitmapSource));
                bmpBitmapEncoder.Save(memorStream);
                result = new System.Drawing.Bitmap(memorStream);
            }
            return result;
        }

        private void ChangeLeftPaletteThumb()
        {
            var point = Mouse.GetPosition(Canvas_LeftPalette as IInputElement);
            this.leftThumbIncrementX = point.X - (this.LeftThumb.ActualWidth / 2);
            this.leftThumbIncrementY = point.Y - (this.LeftThumb.ActualHeight / 2);
            if (point.X >= 5 && point.X <= 195 && point.Y >= 5 && point.Y <= 195)
            {
                Canvas.SetLeft(this.LeftThumb, leftThumbIncrementX);
                Canvas.SetTop(this.LeftThumb, leftThumbIncrementY);
                this.SelectedColor = this.GetPixelColor();
            }
        }

        private void ChangeRightPaletteThumb()
        {
            var point = Mouse.GetPosition(this.Canvas_RightPalette as IInputElement);
            var incrementY = point.Y - (this.RightThumb.ActualHeight / 2);
            if (incrementY >= 0 && incrementY <= 195)
            {
                Canvas.SetTop(this.RightThumb, incrementY);
                var pixel = RightPalette.GetPixel(20, (int)incrementY);
                Rectangle1.Fill = new SolidColorBrush(Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B));
                this.LeftPalette = ConvertBitmapSourceToBitmap(GetTargetBitmap(this.Canvas_LeftPalette));
                this.SelectedColor = this.GetPixelColor();
            }
        }

        private Brush GetPixelColor()
        {
            if (this.leftThumbIncrementX < 0) this.leftThumbIncrementX = 0;
            if (this.leftThumbIncrementY < 0) this.leftThumbIncrementY = 0;
            var pixel = this.LeftPalette.GetPixel((int)this.leftThumbIncrementX, (int)this.leftThumbIncrementY);
            return new SolidColorBrush(Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B));
        }

        private void LeftPaletteMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.ChangeLeftPaletteThumb();
        }

        private void LeftPaletteMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) this.ChangeLeftPaletteThumb();
        }

        private void RightPaletteMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) this.ChangeRightPaletteThumb();
        }

        private void RightPaletteMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.ChangeRightPaletteThumb();
        }
    }
}
