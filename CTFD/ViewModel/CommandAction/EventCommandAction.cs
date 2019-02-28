using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace CTFD.ViewModel.CommandAction
{
    public class EventCommandParameter
    {
        /// <summary>  
        /// 事件触发源  
        /// </summary>  
        public DependencyObject Sender { get; set; }

        /// <summary>  
        /// 事件参数  
        /// </summary>  
        public EventArgs EventArgs { get; set; }
    }

    public class EventCommand : TriggerAction<DependencyObject>
    {
        private string commandName;
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(EventCommand), null);
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(EventCommand), null);

        public string CommandName
        {
            get
            {
                base.ReadPreamble();
                return this.commandName;
            }
            set
            {
                if (this.CommandName != value)
                {
                    base.WritePreamble();
                    this.commandName = value;
                    base.WritePostscript();
                }
            }
        }

        public ICommand Command
        {
            get => (ICommand)base.GetValue(CommandProperty);
            set => base.SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => base.GetValue(CommandParameterProperty);
            set => base.SetValue(CommandParameterProperty, value);
        }

        /// <summary>  
        /// 调用操作。  
        /// </summary>  
        /// <param name="parameter">操作的参数。如果操作不需要参数，则可以将参数设置为空引用。</param>  
        protected override void Invoke(object parameter)
        {
            if (base.AssociatedObject != null)
            {
                ICommand command = this.ResolveCommand();
                var exParameter = new EventCommandParameter { Sender = base.AssociatedObject, EventArgs = parameter as EventArgs };
                if (command != null && command.CanExecute(exParameter)) command.Execute(exParameter);
            }
        }

        private ICommand ResolveCommand()
        {
            var result = default(ICommand);
            if (this.Command != null) result = this.Command;
            else
            {
                if (base.AssociatedObject != null)
                {
                    Type type = base.AssociatedObject.GetType();
                    var propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    //PropertyInfo[] array = propertyInfos;
                    for (int i = 0; i < propertyInfos.Length; i++)
                    {
                        if (typeof(ICommand).IsAssignableFrom(propertyInfos[i].PropertyType) && string.Equals(propertyInfos[i].Name, this.CommandName, StringComparison.Ordinal))
                        {
                            result = (ICommand)propertyInfos[i].GetValue(base.AssociatedObject, null);
                        }
                    }
                }
            }
            return result;
        }
    }
}
