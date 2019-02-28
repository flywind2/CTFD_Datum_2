using CTFD.ViewModel.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CTFD.View.Control
{
    public class VerifyTextBox:TextBox
    {
        Binding binding = new Binding();

        //public string Text
        //{
        //    get { return (string)GetValue(TextProperty); }
        //    set { SetValue(TextProperty, value); }
        //}
        //public static readonly DependencyProperty TextProperty =
        //    DependencyProperty.Register(nameof(Text), typeof(string), typeof(VerifyTextBox), new PropertyMetadata(string.Empty));

        public ValidationRule ValidationRule
        {
            get { return (ValidationRule)GetValue(ValidationRuleProperty); }
            set { SetValue(ValidationRuleProperty, value); }
        }
        public static readonly DependencyProperty ValidationRuleProperty =
            DependencyProperty.Register(nameof(ValidationRule), typeof(ValidationRule), typeof(VerifyTextBox), new PropertyMetadata(null,new PropertyChangedCallback(Callback_ValidationRule)));

        public VerifyTextBox()
        {
            binding.Source = this;
            binding.Path = new PropertyPath(nameof(Text));
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.NotifyOnValidationError = true;
          
            binding.ValidatesOnDataErrors = true;
            this.SetBinding(TextBox.TextProperty, binding);
        }

        private static void Callback_ValidationRule(DependencyObject send, DependencyPropertyChangedEventArgs e)
        {
            var textBoxVal = send as VerifyTextBox;
            textBoxVal.binding.ValidationRules.Clear();
            textBoxVal.binding.ValidationRules.Add(e.NewValue as ValidationRule);
        }
    }
}
