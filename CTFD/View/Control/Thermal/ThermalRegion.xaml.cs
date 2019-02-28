using CTFD.Global.Common;
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

namespace CTFD.View.Control.Thermal
{
    /// <summary>
    /// ThermalRegion.xaml 的交互逻辑
    /// </summary>
    public partial class ThermalRegion : UserControl
    {
        private int selectedIndex = 0;

        public ThermalRegion() { InitializeComponent(); }

        private ThermalCell GetThermalCell(int index) => this.StackPanel1.Children[index] as ThermalCell;

        private ThermalCell GetLastThermalCell() => this.StackPanel1.Children[this.StackPanel1.Children.Count - 1] as ThermalCell;

        private void AddThermalCell(object sender, RoutedEventArgs e)
        {
            var newThermalCell = new ThermalCell { Margin = new Thickness(-1, 0, 0, 0) };

            if (this.StackPanel1.Children.Count == 0) newThermalCell.SetValue(MarginProperty, new Thickness());

            newThermalCell.ThermalChanged += ThermalChanged;

            newThermalCell.MouseLeftButtonDown += ThermalCellSelected;

            if (this.StackPanel1.Children.Count > 0) this.StackPanel1.Children.Insert(this.selectedIndex + 1, newThermalCell);

            else this.StackPanel1.Children.Add(newThermalCell);

            if (this.StackPanel1.Children.Count > 1) General.SetBinding(this.GetThermalCell(this.selectedIndex), newThermalCell);

            if (this.selectedIndex < this.StackPanel1.Children.Count - 2) General.SetBinding(newThermalCell, this.GetThermalCell(this.selectedIndex + 2));

            this.selectedIndex = this.StackPanel1.Children.Count - 1;

            this.ResetSelection();
        }

        private void RemoveThermalCell(object sender, RoutedEventArgs e)
        {
            this.RemoveThermalCell();
        }

        private void RemoveThermalCell()
        {
            if (this.StackPanel1.Children.Count > 0)
            {
                var selectedThermalCell = this.GetThermalCell(this.selectedIndex);
                selectedThermalCell.ThermalChanged -= ThermalChanged;
                selectedThermalCell.MouseLeftButtonDown -= ThermalCellSelected;
                if (this.selectedIndex > 0 && this.selectedIndex < this.StackPanel1.Children.Count - 1) General.SetBinding(this.GetThermalCell(this.selectedIndex - 1), this.GetThermalCell(this.selectedIndex + 1));
                else if (this.selectedIndex == 0 && this.StackPanel1.Children.Count > 1) this.GetThermalCell(this.selectedIndex + 1).GetFirstThermalUnit().Y1 = 300D;
                this.StackPanel1.Children.Remove(selectedThermalCell);
                if (this.StackPanel1.Children.Count == 1) this.selectedIndex = 0;
                else this.selectedIndex = this.StackPanel1.Children.Count - 1;
            }
        }

        private void SetBindingPrevious(ThermalCell thermalCell)
        {
            var thermalCellIndex = this.StackPanel1.Children.IndexOf(thermalCell);
            if (thermalCellIndex != 0)
            {
                var previousThermalCell = this.GetThermalCell(thermalCellIndex - 1);
                General.SetBinding(previousThermalCell, thermalCell);
            }
        }

        private void SetBindingNext(ThermalCell thermalCell)
        {
            var thermalCellIndex = this.StackPanel1.Children.IndexOf(thermalCell);
            if (thermalCellIndex != this.StackPanel1.Children.Count - 1)
            {
                General.SetBinding(thermalCell, this.GetThermalCell(thermalCellIndex + 1));
            }
        }

        private void ResetSelection()
        {
            foreach (var item in this.StackPanel1.Children)
            {
                if (item is ThermalCell thermalCell) thermalCell.SetSelection(false);
            }
        }

        private void ThermalChanged(object sender, ThermalChangeArgs e)
        {
            var thermalCell = sender as ThermalCell;
            switch (e.ThermalChange)
            {
                case ThermalChange.RemoveThermalCell:
                {
                    this.selectedIndex = this.StackPanel1.Children.IndexOf(thermalCell);
                    this.RemoveThermalCell();
                    break;
                }
                case ThermalChange.ChangeLastThermalUnit:
                {
                    this.SetBindingNext(thermalCell);
                    break;
                }
                case ThermalChange.ChangeFirstThermalUnit:
                {
                    this.SetBindingPrevious(thermalCell);
                    break;
                }
            }
        }

        private void ThermalCellSelected(object sender, MouseButtonEventArgs e)
        {
            this.ResetSelection();
            var selectedThermalCell = sender as ThermalCell;
            selectedThermalCell.SetSelection(true);
            this.selectedIndex = this.StackPanel1.Children.IndexOf(selectedThermalCell);
        }
    }
}
