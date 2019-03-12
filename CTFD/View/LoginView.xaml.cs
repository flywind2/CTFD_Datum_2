using CTFD.Global.Common;
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

namespace CTFD.View
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            (this.DataContext as ViewModel.LoginViewModel).ViewChanged += (s, args) =>
            {
                var user = General.WorkingData.Configuration.Accounts.FirstOrDefault(o => o.UserName == this.TextBox_UserName.Text);
                if (user != null)
                {
                    if (user.Password == this.TextBox_Password.Password)
                    {
                        General.WorkingData.Configuration.Account = user;
                        General.RaiseGlobalHandler(GlobalEvent.ShowWorkingView, null);
                        this.ChangeErrorMessage(Visibility.Hidden);
                        this.TextBox_UserName.IsEnabled = false;
                        this.TextBox_Password.IsEnabled = false;
                        this.Button_Login.IsEnabled = false;
                        this.Button_Logout.IsEnabled = true;
                        this.Button_Work.Visibility = Visibility.Visible;

                    }
                    else this.ChangeErrorMessage(Visibility.Visible, 2);
                }
                else
                {
                    this.ChangeErrorMessage(Visibility.Visible, 1);
                }
            };
        }

        

        private void ChangeErrorMessage(Visibility visibility, int index = 0)
        {
            switch (index)
            {
                case 1:
                {
                    if (this.TextBlock_UserNameError != null) this.TextBlock_UserNameError.Visibility = visibility;
                    break;
                }
                case 2:
                {
                    if (this.TextBlock_PassworkError != null) this.TextBlock_PassworkError.Visibility = visibility;
                    break;
                }
                default:
                {
                    if (this.TextBlock_UserNameError != null) this.TextBlock_UserNameError.Visibility = visibility;
                    if (this.TextBlock_PassworkError != null) this.TextBlock_PassworkError.Visibility = visibility;
                    break;
                }
            }
        }

        private void ChangeErrorIcon(Visibility visibility, int index = 0)
        {
            switch (index)
            {
                case 1: { this.Button_RemovingUserName.Visibility = visibility; break; }
                case 2: { this.Button_RemovingPassword.Visibility = visibility; break; }
                default:
                {
                    this.Button_RemovingUserName.Visibility = visibility;
                    this.Button_RemovingPassword.Visibility = visibility;
                    break;
                }
            }
        }


        private void TextBox_UserName_MouseEnter(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(this.TextBox_UserName.Text) == false) this.ChangeErrorIcon(Visibility.Visible, 1);
        }

        private void TextBox_UserName_MouseLeave(object sender, MouseEventArgs e)
        {
            this.ChangeErrorIcon(Visibility.Hidden, 1);
        }

        private void TextBox_UserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ChangeErrorMessage(Visibility.Hidden, 1);
            if (string.IsNullOrEmpty(this.TextBox_UserName.Text)) this.ChangeErrorIcon(Visibility.Hidden, 1);
        }

        private void Button_RemovingUserName_Click(object sender, RoutedEventArgs e)
        {
            this.TextBox_UserName.Clear();
        }


        private void TextBox_Password_MouseEnter(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(this.TextBox_Password.Password) == false) this.ChangeErrorIcon(Visibility.Visible, 2);
        }

        private void TextBox_Password_MouseLeave(object sender, MouseEventArgs e)
        {
            this.ChangeErrorIcon(Visibility.Hidden, 2);
        }

        private void TextBox_Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.ChangeErrorMessage(Visibility.Hidden, 2);
            if (string.IsNullOrEmpty(this.TextBox_Password.Password)) this.ChangeErrorIcon(Visibility.Hidden, 2);
        }

        private void Button_RemovingPassword_Click(object sender, RoutedEventArgs e)
        {
            this.TextBox_Password.Clear();
        }

        private void Button_Logout_Click(object sender, RoutedEventArgs e)
        {
            this.TextBox_UserName.IsEnabled = true;
            this.TextBox_Password.IsEnabled = true;
            this.Button_Login.IsEnabled = true;
            this.Button_Logout.IsEnabled = false;
            this.TextBox_UserName.Clear();
            this.TextBox_Password.Clear();
            this.Button_Work.Visibility = Visibility.Hidden;
        }

        private void Button_Work_Click(object sender, RoutedEventArgs e)
        {
            General.RaiseGlobalHandler(GlobalEvent.ShowWorkingView, null);
        }
    }
}
