using CTFD.Global.Common;
using CTFD.Model.RuntimeData;
using CTFD.ViewModel.Base;
using System;
using System.Linq;
using System.Text;

namespace CTFD.ViewModel.Monitor
{
    public partial class MonitorViewModel
    {
        public Model.RuntimeData.Setup Configuration => General.WorkingData.Configuration;

        public string[] Roles => new string[] { "","操作员", "管理员", "工程师" };

        private Account selectedAccount;
        public Account SelectedAccount
        {
            get => this.selectedAccount;
            set
            {
                this.selectedAccount = value;
                this.DeleteAccount.RaiseCanExecuteChanged();
            }
        }

        public Account CurrentAccount { get; set; } = new Account();

        public RelayCommand AddAccount { get; private set; }

        public RelayCommand DeleteAccount { get; private set; }

        public RelayCommand ChangeText => new RelayCommand(() => this.AddAccount.RaiseCanExecuteChanged());

        private void ExecuteAddAccount()
        {
            if (this.Configuration.Accounts.FirstOrDefault(o => o.UserName == CurrentAccount.UserName) != null)
            {
                General.ShowToast("该用户名已被注册");
            }
            else
            {
                this.Configuration.Accounts.Add(new Account(this.CurrentAccount.UserName, this.CurrentAccount.Password, this.CurrentAccount.Role));
                this.WriteSetupFile();
                this.CurrentAccount.Clear();
                this.AddAccount.RaiseCanExecuteChanged();
            }
        }

        private bool CanExecuteAddAccount()
        {
            var current = this.CurrentAccount;
            var result = false;
            if (!string.IsNullOrEmpty(current.UserName) && !string.IsNullOrEmpty(current.Password) && !string.IsNullOrEmpty(current.Role)) result = true;
            return result;
        }

        private void ExecuteDeleteAccount()
        {
            if (this.SelectedAccount != null)
            {
                this.Configuration.Accounts.Remove(this.SelectedAccount);
                this.WriteSetupFile();
                General.ShowToast("成功删除用户");
            }
        }

        private bool CanExecuteDeleteAccount() => this.SelectedAccount == null ? false : true;

        private void AssignValue(Account from, Account to)
        {
            if (from != null && to != null)
            {
                to.UserName = from.UserName;
                to.Password = from.Password;
                to.Role = from.Role;
            }
        }
    }
}
