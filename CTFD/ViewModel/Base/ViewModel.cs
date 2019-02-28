using CTFD.Model.Base;
using System;

namespace CTFD.ViewModel.Base
{
    public class ViewModel : Notify
    {
        public event EventHandler<object> ViewChanged;

        protected virtual void OnViewChanged(object obj = null)
        {
            this.ViewChanged?.Invoke(this, obj);
        }
    }
}
