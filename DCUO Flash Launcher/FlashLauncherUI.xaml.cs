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
using System.Threading;
using Serilog;
using Serilog.Configuration;

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
        public ObservableCollection<Account> Accounts { get; set; }

        /// <summary>
        /// Manager for User Accounts
        /// </summary>
        private AccountManager AccMgr { get; set; }

        /// <summary>
        /// DCUO API
        /// </summary>
        private API DcuoAPI { get; set; }

        /// <summary>
        /// initiate the launcher
        /// </summary>
        public FlashLauncherUI()
        {
            // init variables
            DcuoAPI = new API();
            AccMgr = new AccountManager();
            Accounts = new ObservableCollection<Account>();

            // start the Logger
            Log.Logger = new LoggerConfiguration().WriteTo.File(@".\debug.log").CreateLogger();

            // init GUI and set DataContext to bind to account list
            InitializeComponent();
            DataContext = Accounts;

            // load account list
            AccMgr.LoadFromDatabase();
            if (AccMgr.accounts.Count > 0)
            {
                foreach (Account account in AccMgr.accounts)
                {
                    Accounts.Add(account);
                }
            }

            // load region
            RegionSettings region = new RegionSettings();
            region.LoadSettings();
            switch (region.CurrentSelection)
            {
                case "EU":
                    ComboBox_RegionSelection.SelectedIndex = 0;
                    break;
                case "US":
                    ComboBox_RegionSelection.SelectedIndex = 1;
                    break;
                default:
                    break;
            }
        }

        private void Button_AddAccount_Click(object sender, RoutedEventArgs e)
        {
            Grid_AddAccount.Visibility = Visibility.Collapsed;
            Grid_Login.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Process Username and Password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TextBox_Username.Text) || !String.IsNullOrEmpty(PasswordBox_Password.Password))
            {
                Progressbar_Login.Visibility = Visibility.Visible;
                Thread.Sleep(1000);
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
                    Progressbar_Login.Visibility = Visibility.Collapsed;
                }
                Progressbar_Login.Visibility = Visibility.Collapsed;
            }
        }

        private void Button_Play_Click(object sender, RoutedEventArgs e)
        {
            if (Accounts.Count > 0)
            {
                if (!(ListBox_AccountList.SelectedIndex == -1))
                {
                    DcuoAPI.SetAccount(Accounts[ListBox_AccountList.SelectedIndex]);
                    Debug.WriteLine("Login start");
                    DcuoAPI.Login();
                    Debug.WriteLine("Login done");
                    if (!DcuoAPI.IsLoggedIn)
                    {
                        Debug.WriteLine("Was unable to Login");
                    }
                    DcuoAPI.GetLaunchArgs();
                    DcuoAPI.LaunchGame();
                    //bot.LaunchGame(Accounts[ListBox_AccountList.SelectedIndex]);
                }
            }
        }

        private void Button_EditAccount_Click(object sender, RoutedEventArgs e)
        {
            EditAccount();
        }

        private void Button_DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Hotkey functions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_AccountList_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    Window delete  = new Window();
                    AccMgr.accounts.Remove(Accounts[ListBox_AccountList.SelectedIndex]);
                    AccMgr.SaveToDatabase();
                    Accounts.Clear();
                    if (AccMgr.accounts.Count > 0)
                    {
                        foreach (Account account in AccMgr.accounts)
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

        /// <summary>
        /// Exit and close the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            Log.CloseAndFlush();
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
        /// Calls the EditAccount() function from GUI
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
                AccMgr.accounts[ListBox_AccountList.SelectedIndex].Username = edit.username;
                AccMgr.accounts[ListBox_AccountList.SelectedIndex].Password = edit.password;
                AccMgr.SaveToDatabase();
                Accounts.Clear();
                if (AccMgr.accounts.Count > 0)
                {
                    foreach (Account account in AccMgr.accounts)
                    {
                        Accounts.Add(account);
                    }
                }
            }
        }

        /// <summary>
        /// Move selected account up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Move selected account down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Clone selected account above
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_CloneAbove_Click(object sender, RoutedEventArgs e)
        {
            Accounts.Add(Accounts[ListBox_AccountList.SelectedIndex]);
            Accounts.Move(ListBox_AccountList.SelectedIndex + 1, ListBox_AccountList.SelectedIndex);

            AccMgr.accounts.Clear();
            foreach (Account acc in Accounts)
            {
               AccMgr.accounts.Add(acc);
            }
            AccMgr.SaveToDatabase();
        }

        /// <summary>
        /// Clone selected account below
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_CloneBelow_Click(object sender, RoutedEventArgs e)
        {
            Accounts.Add(Accounts[ListBox_AccountList.SelectedIndex]);

            AccMgr.accounts.Clear();
            foreach (Account acc in Accounts)
            {
                AccMgr.accounts.Add(acc);
            }
            AccMgr.SaveToDatabase();
        }


        /// <summary>
        /// Save the new region based on selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_RegionSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RegionSettings region = new RegionSettings();
            switch (ComboBox_RegionSelection.SelectedIndex)
            {
                case 0:
                    region.CurrentSelection = "EU";
                    break;
                case 1:
                    region.CurrentSelection = "US";
                    break;
                default:
                    break;
            }
            region.WriteSettings();
        }
    }
}
