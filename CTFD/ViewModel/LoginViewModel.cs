using CTFD.Global.Common;
using CTFD.Model.RuntimeData;
using CTFD.ViewModel.Base;

namespace CTFD.ViewModel
{
    public class LoginViewModel : Base.ViewModel
    {
        public Account Account = new Account();

        public RelayCommand Login => new RelayCommand(() => General.RaiseGlobalHandler(GlobalEvent.ShowWorkingView, null));

        public RelayCommand Clear => new RelayCommand(()=> this.OnViewChanged());

        public LoginViewModel() { }
    }
}
