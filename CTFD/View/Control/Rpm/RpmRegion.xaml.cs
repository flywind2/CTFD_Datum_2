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

namespace CTFD.View.Control.Rpm
{
    /// <summary>
    /// RpmRegion.xaml 的交互逻辑
    /// </summary>
    public partial class RpmRegion : UserControl
    {
        private int selectedIndex = 0;
        public RpmRegion()
        {
            InitializeComponent();
        }

        private void AddRpmUnit(object sender, RoutedEventArgs e)
        {
            var newRpmUnit = new RpmUnit();

            if (this.StackPanel1.Children.Count == 0) this.StackPanel1.Children.Add(newRpmUnit);
            else this.StackPanel1.Children.Insert(this.selectedIndex + 1, newRpmUnit);
            newRpmUnit.MouseLeftButtonUp += RpmUnitSelected;
            this.ResetSelection();
            this.ResetSelectedIndex();
        }

        private void RpmUnitSelected(object sender, MouseButtonEventArgs e)
        {
            this.ResetSelection();
            var selectedRpmUnit = sender as RpmUnit;
            selectedRpmUnit.SetSelection(true);
            this.selectedIndex = this.StackPanel1.Children.IndexOf(selectedRpmUnit);
        }

        private void RemoveRpmUnit(object sender, RoutedEventArgs e)
        {
            if (this.StackPanel1.Children.Count > 0)
            {
                var selectedRpmUnit = this.StackPanel1.Children[this.selectedIndex];
                selectedRpmUnit.MouseLeftButtonUp -= RpmUnitSelected;
                this.StackPanel1.Children.Remove(selectedRpmUnit);
                this.ResetSelection();
                this.ResetSelectedIndex();
            }
        }

        private void ResetSelection()
        {
            foreach (var item in this.StackPanel1.Children)
            {
                if (item is RpmUnit rpmUnit)
                {
                    rpmUnit.SetSelection(false);
                }
            }
        }

        private void ResetSelectedIndex() => this.selectedIndex = this.StackPanel1.Children.Count - 1;

    }
}
