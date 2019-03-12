using CTFD.Global.Common;
using CTFD.Model.RuntimeData;
using CTFD.ViewModel.Base;
using System.Linq;

namespace CTFD.ViewModel
{
    public class LoginViewModel : Base.ViewModel
    {
        public Account Account => General.WorkingData.Configuration.Account;

        public RelayCommand Login { get; private set; }

        public LoginViewModel()
        {
            this.Login = new RelayCommand(() => this.OnViewChanged());
            this.Account.Password = string.Empty;
        }
    }
}
