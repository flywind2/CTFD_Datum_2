using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace CTFD.ViewModel.Behavior
{
    public class BehaviorBase : Behavior<UIElement>
    {
        public DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T)) source = VisualTreeHelper.GetParent(source);
            return source;
        }
    }
}
