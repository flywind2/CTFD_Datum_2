using CTFD.Global.Common;
using CTFD.Model.RuntimeData;
using CTFD.View.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CTFD.ViewModel.Behavior
{
    public class SampleMultiSelect : MultiSelection
    {
        public override object ViewModel
        {
            get { return GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(object), typeof(SampleMultiSelect), new PropertyMetadata(null));

        public object View
        {
            get { return (object)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }
        public static readonly DependencyProperty ViewProperty =
            DependencyProperty.Register(nameof(View), typeof(object), typeof(SampleMultiSelect), new PropertyMetadata(null));

        protected override HitTestResultCallback MouseDownHitTestResultCallback => new HitTestResultCallback(result => { return HitTestResultBehavior.Continue; });

        protected override HitTestResultCallback MouseMoveHitTestResultCallback => new HitTestResultCallback(result =>HitTestResultBehavior.Continue);

        protected override HitTestResultCallback MouseUpHitTestResultCallback => new HitTestResultCallback(result =>
        {
            var hitTestResultBehavior = HitTestResultBehavior.Continue;
            var obj = result.VisualHit;
            if (obj != null)
            {
                if (obj is Ellipse ellipse)
                {
                    if (Panel.GetZIndex(ellipse) == 10)
                    {
                        if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) ((Base.ISample)this.ViewModel).ResetSelection();
                        var sample = ellipse.DataContext as Sample;
                        sample.IsSelected = !sample.IsSelected;
                        hitTestResultBehavior = HitTestResultBehavior.Stop;
                    }
                }
            }
            else hitTestResultBehavior = HitTestResultBehavior.Stop;
            return hitTestResultBehavior;
        });
    }
}
