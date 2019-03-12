using CTFD.Global.Common;
using CTFD.Model.RuntimeData;
using CTFD.ViewModel.Base;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace CTFD.ViewModel.Setup
{
    public class SettingViewModel : Base.ViewModel
    {
        public string GroupName { get; set; }
        public string ProjectName { get; set; }
        public string Detection { get; set; }

        public string[] Roles => new string[] { "操作员","管理员","工程师"};

        public Sample[] Detections { get; private set; }

        public Model.RuntimeData.Setup Configuration => General.WorkingData.Configuration;

        private Account selectedAccount = new Account();
        public Account SelectedAccount
        {
            get => this.selectedAccount;
            set
            {
                this.selectedAccount = value;
                this.AssignValue(this.selectedAccount, this.CurrentAccount);
                this.RaisePropertyChanged(nameof(this.CurrentAccount));
            }
        }

        public Account CurrentAccount { get; set; } = new Account();

        private bool isPopupColorSelectionBox;
        public bool IsPopupColorSelectionBox
        {
            get { return isPopupColorSelectionBox; }
            set
            {
                isPopupColorSelectionBox = value;
                this.RaisePropertyChanged(nameof(this.IsPopupColorSelectionBox));
            }
        }

        public RelayCommand AddDetection => new RelayCommand(() =>
        {
           
            this.WriteSetupFile();
        });

        public RelayCommand AddAccount => new RelayCommand(() =>
        {

            if (this.Configuration.Accounts.FirstOrDefault(o => o.UserName == CurrentAccount.UserName) != null)
            {
                MessageBox.Show("该用户名已被注册");
            }
            else
            {
                this.Configuration.Accounts.Add(new Account(this.CurrentAccount.UserName, this.CurrentAccount.Password, this.CurrentAccount.Role, this.CurrentAccount.Remark));
                this.WriteSetupFile();
            }
        });

        public RelayCommand EditAccount => new RelayCommand(() =>
        {
            if (this.Configuration.Accounts.FirstOrDefault(o => o.UserName == CurrentAccount.UserName) != null)
            {
                MessageBox.Show("该用户名已被注册");
            }
            else
            {
                if (this.SelectedAccount != null && this.SelectedAccount.UserName != null)
                {
                    this.AssignValue(this.CurrentAccount, this.SelectedAccount);
                    this.SelectedAccount?.RaiseProperties();
                    MessageBox.Show("成功修改用户");
                }
            }
            //this.RaisePropertyChanged(nameof(this.SelectedAccount));
        });

        public RelayCommand DeleteAccount => new RelayCommand(() =>
        {
            if (this.SelectedAccount != null && this.SelectedAccount.UserName != null)
            {
                this.Configuration.Accounts.Remove(this.SelectedAccount);
                if (this.Configuration.Accounts.Count > 0) this.SelectedAccount = this.Configuration.Accounts[this.Configuration.Accounts.Count - 1];
                else this.SelectedAccount = default(Account);
                this.WriteSetupFile();
                MessageBox.Show("成功删除用户");
            }
        });

        public RelayCommand SelectColor => new RelayCommand(() => this.IsPopupColorSelectionBox = true);

        public RelayCommand ReConnectServer => new RelayCommand(() =>
        {
            General.RaiseGlobalHandler(GlobalEvent.ResetTcpServer);
            this.WriteSetupFile();
        });

        public SettingViewModel()
        {
            this.Detections = new Sample[32];

        }

        private void AssignValue(Account from, Account to)
        {
            if (from != null && to != null)
            {
                to.UserName = from.UserName;
                to.Password = from.Password;
                to.Role = from.Role;
                to.Remark = from.Remark;
            }
        }

        private void WriteSetupFile()
        {
            General.WriteJsonFile((object)Configuration, $"{Environment.CurrentDirectory}{Properties.Resources.SetupFilePath}", Encoding.Default);
        }
    }
}
