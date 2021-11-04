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
using System.Windows.Shapes;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace FlashLauncher
{
    /// <summary>
    /// Interaktionslogik für FlashLauncherUI.xaml
    /// </summary>
    public partial class FlashLauncherUI : Window
    {
        /// <summary>
        /// list of all acounts that will be shown on the ui
        /// </summary>
        private ObservableCollection<Account> Accounts = new();

        public FlashLauncherUI()
        {
            InitializeComponent();
            DataContext = Accounts;
            AccountManager am = new();
            am.LoadFromDatabase();
            if (am.accounts.Count > 0)
            {
                foreach (Account account in am.accounts)
                {
                    Accounts.Add(account);
                }
            }
        }

        private void Button_AddAccount_Click(object sender, RoutedEventArgs e)
        {
            Grid_AddAccount.Visibility = Visibility.Collapsed;
            Grid_Login.Visibility = Visibility.Visible;
        }

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TextBox_Username.Text) || !String.IsNullOrEmpty(PasswordBox_Password.Password))
            {
                Account account = new(TextBox_Username.Text, PasswordBox_Password.Password);
                WebBot bot = new();
                bool isLoggedIn = bot.Login(account);
                if (isLoggedIn)
                {
                    AccountManager am = new AccountManager();
                    Accounts.Add(account);
                    am.CreateNewAccount(account);

                    // clear data
                    TextBox_Username.Text = "";
                    PasswordBox_Password.Password = "";
                    Button_Login.Background = new SolidColorBrush(Colors.White);

                    Grid_Login.Visibility = Visibility.Collapsed;
                    Grid_AddAccount.Visibility = Visibility.Visible;
                }
                else
                {
                    Button_Login.Background = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private void Button_Play_Click(object sender, RoutedEventArgs e)
        {
            if (Accounts.Count > 0)
            {
                if (!(ListBox_AccountList.SelectedIndex == -1))
                {
                    WebBot bot = new();
                    bot.Login(Accounts[ListBox_AccountList.SelectedIndex]);
                    bot.LaunchGame(Accounts[ListBox_AccountList.SelectedIndex]);
                }
            }
        }
    }
}
