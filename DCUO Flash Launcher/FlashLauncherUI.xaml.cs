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
        private AccountManager am = new();

        public FlashLauncherUI()
        {
            InitializeComponent();
            DataContext = Accounts;
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

        private void Button_EditAccount_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_DeleteAccount_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListBox_AccountList_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    Window delete  = new Window();
                    am.accounts.Remove(Accounts[ListBox_AccountList.SelectedIndex]);
                    am.SaveToDatabase();
                    Accounts.Clear();
                    if (am.accounts.Count > 0)
                    {
                        foreach (Account account in am.accounts)
                        {
                            Accounts.Add(account);
                        }
                    }
                    break;
                case Key.E:
                    EditAccount();
                    break;

                default:
                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ListBox_AccountList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Button_Play.Visibility == Visibility.Collapsed)
            {
                Button_Play.Visibility = Visibility.Visible;
                Button_EditAccount.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Calls the EditAccount() function 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditAccount(object sender, MouseButtonEventArgs e)
        {
            EditAccount();
        }

        /// <summary>
        /// Open a edit account window
        /// </summary>
        private void EditAccount()
        {
            if (ListBox_AccountList.SelectedIndex != -1)
            {
                EditAccount edit = new(
                    Accounts[ListBox_AccountList.SelectedIndex].Username,
                    Accounts[ListBox_AccountList.SelectedIndex].Password);
                edit.Owner = this;
                edit.ShowDialog();
                am.accounts[ListBox_AccountList.SelectedIndex].Username = edit.username;
                am.accounts[ListBox_AccountList.SelectedIndex].Password = edit.password;
                am.SaveToDatabase();
                Accounts.Clear();
                if (am.accounts.Count > 0)
                {
                    foreach (Account account in am.accounts)
                    {
                        Accounts.Add(account);
                    }
                }
            }
        }

        private void Button_MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_AccountList.SelectedIndex - 1 > -1)
            {
                int newIndex = ListBox_AccountList.SelectedIndex - 1;
                if (newIndex >= 0)
                {
                    Accounts.Move(ListBox_AccountList.SelectedIndex, newIndex);
                    ListBox_AccountList.SelectedIndex = newIndex;
                }
                else
                {
                    Accounts.Move(ListBox_AccountList.SelectedIndex, Accounts.Count -1);
                    ListBox_AccountList.SelectedIndex = Accounts.Count - 1;
                }
            }
        }

        private void Button_MoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_AccountList.SelectedIndex > 1)
            {
                int newIndex = ListBox_AccountList.SelectedIndex + 1;
                if (newIndex <= Accounts.Count - 1)
                {
                    Accounts.Move(ListBox_AccountList.SelectedIndex, newIndex);
                    ListBox_AccountList.SelectedIndex = newIndex;
                }
                else
                {
                    Accounts.Move(ListBox_AccountList.SelectedIndex, 0);
                    ListBox_AccountList.SelectedIndex = 0;
                }
            }
        }

        private void Button_CloneAbove_Click(object sender, RoutedEventArgs e)
        {
            Accounts.Add(Accounts[ListBox_AccountList.SelectedIndex]);
            Accounts.Move(ListBox_AccountList.SelectedIndex + 1, ListBox_AccountList.SelectedIndex);

            am.accounts.Clear();
            foreach (Account acc in Accounts)
            {
               am.accounts.Add(acc);
            }
            am.SaveToDatabase();
        }

        private void Button_CloneBelow_Click(object sender, RoutedEventArgs e)
        {
            Accounts.Add(Accounts[ListBox_AccountList.SelectedIndex]);

            am.accounts.Clear();
            foreach (Account acc in Accounts)
            {
                am.accounts.Add(acc);
            }
            am.SaveToDatabase();
        }
    }
}
