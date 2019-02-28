using System;
using System.Collections.Generic;
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
using System.Linq;
using CTFD.Global.Common;

namespace CTFD.View.Control.Thermal
{
    /// <summary>
    /// ThermalCell.xaml 的交互逻辑
    /// </summary>
    public partial class ThermalCell : UserControl
    {
        private int test = 0;
        private int insertIndex = 0;
        public event EventHandler<ThermalChangeArgs> ThermalChanged;

        public Brush BackgroundColor
        {
            get { return (Brush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Brush), typeof(ThermalCell), new PropertyMetadata(Brushes.White));

        public int Step
        {
            get { return (int)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register(nameof(Step), typeof(int), typeof(ThermalCell), new PropertyMetadata(1));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register(nameof(Name), typeof(string), typeof(ThermalCell), new PropertyMetadata(string.Empty));

        public int CycleCount
        {
            get { return (int)GetValue(CycleCountProperty); }
            set { SetValue(CycleCountProperty, value); }
        }
        public static readonly DependencyProperty CycleCountProperty =
            DependencyProperty.Register(nameof(CycleCount), typeof(int), typeof(ThermalCell), new PropertyMetadata(1));

        public ThermalCell()
        {
            InitializeComponent();
            this.DataContext = this;
            this.AddThermalUnit(new ThermalUnit());
        }

        protected virtual void OnThermalChanged(ThermalChange thermalChange)
        {
            this.ThermalChanged?.Invoke(this, new ThermalChangeArgs(thermalChange));
        }

        private void AddThermalUnit(ThermalUnit thermalUnit)
        {
            this.test++;
            thermalUnit.test = this.test;
            thermalUnit.PreviewMouseLeftButtonDown += ThermalUnit_PreviewMouseLeftButtonDown;

            if (this.StackPanel1.Children.Count > 0) this.StackPanel1.Children.Insert(this.insertIndex + 1, thermalUnit);
            else this.StackPanel1.Children.Add(thermalUnit);
        }

        private void ThermalUnit_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var currentThermalUnit = sender as ThermalUnit;
            foreach (var item in this.StackPanel1.Children)
            {
                if (item is ThermalUnit _thermalUnit)
                {
                    if (_thermalUnit == currentThermalUnit)
                    {
                        _thermalUnit.SetSelection(true);
                        this.insertIndex = this.StackPanel1.Children.IndexOf(_thermalUnit);
                    }
                    else _thermalUnit.SetSelection(false);
                }
            }
        }

        private void AddThermalUnit(object sender, RoutedEventArgs e)
        {
            var newThermalUnit = new ThermalUnit();

            this.AddThermalUnit(newThermalUnit);

            if (this.StackPanel1.Children.Count > 1) General.SetBinding(this.GetThermalUnit(this.insertIndex), newThermalUnit);

            if (this.insertIndex < this.StackPanel1.Children.Count - 2) General.SetBinding(newThermalUnit, this.GetThermalUnit(this.insertIndex + 2));

            this.insertIndex = this.StackPanel1.Children.Count - 1;

            if (newThermalUnit == this.GetLastThermalUnit()) this.OnThermalChanged(ThermalChange.ChangeLastThermalUnit);
        }

        private void RemoveThermalUnit(object sender, RoutedEventArgs e)
        {
            var selectedThermalUnit = this.GetThermalUnit(this.insertIndex);

            selectedThermalUnit.PreviewMouseLeftButtonDown -= ThermalUnit_PreviewMouseLeftButtonDown;

            var isRemoveLastThermalUnit = false;
            var isRemoveFirstThermalUnit = false;
            if (this.StackPanel1.Children.Count == 1) this.OnThermalChanged(ThermalChange.RemoveThermalCell);
            else
            {
                if (this.insertIndex == 0) { this.GetThermalUnit(this.insertIndex + 1).Y1 = 300D; isRemoveFirstThermalUnit = true; }

                else if (this.insertIndex < this.StackPanel1.Children.Count - 1) General.SetBinding(this.GetThermalUnit(this.insertIndex - 1), this.GetThermalUnit(this.insertIndex + 1));

                else isRemoveLastThermalUnit = true;
            }
            this.StackPanel1.Children.Remove(selectedThermalUnit);

            if (isRemoveLastThermalUnit) this.OnThermalChanged(ThermalChange.ChangeLastThermalUnit);

            if (isRemoveFirstThermalUnit) this.OnThermalChanged(ThermalChange.ChangeFirstThermalUnit);

            this.insertIndex = this.StackPanel1.Children.Count - 1;
        }

        public void SetSelection(bool isSelected)
        {
            this.BackgroundColor = isSelected ? General.WathetColor2 : Brushes.White;
        }

        public ThermalUnit GetFirstThermalUnit() => this.StackPanel1.Children[0] as ThermalUnit;

        public ThermalUnit GetLastThermalUnit() => this.StackPanel1.Children[this.StackPanel1.Children.Count - 1] as ThermalUnit;

        public ThermalUnit GetThermalUnit(int index) => this.StackPanel1.Children[index] as ThermalUnit;
    }
}
