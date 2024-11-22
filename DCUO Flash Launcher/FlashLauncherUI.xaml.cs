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
using System.Net.Http;
using System.Net;
using System.Security.Policy;
using Windows.Media.Protection.PlayReady;
using System.Runtime.CompilerServices;

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
        /// Region selection for Server choice
        /// </summary>
        private RegionSettings Region { get; set; }

        /// <summary>
        /// initiate the launcher
        /// </summary>
        public FlashLauncherUI()
        {
            Debug.WriteLine("[ FlashLauncherUI:Init ] Program started");

            Debug.Write("[ FlashLauncherUI:Init ] Init Variables... ");
            // init variables
            API2.Cookies = new CookieContainer();
            API2.Handler = new HttpClientHandler();
            API2.Handler.CookieContainer = API2.Cookies;
            API2.Client = new HttpClient(API2.Handler, false);
            API2.IsLoggedIn = false;
            API2.LaunchArgs = "";
            API2.dcuoLaunchmefirstPath = new(@"C:\Users\Public\Daybreak Game Company\Installed Games\DC Universe Online\UNREAL3\BINARIES\WIN32\LAUNCHMEFIRST.EXE");
 
            AccMgr = new AccountManager();
            Accounts = new ObservableCollection<Account>();

            // start the Logger
            Log.Logger = new LoggerConfiguration().WriteTo.File(@".\debug.log").CreateLogger();

            Debug.WriteLine("done");


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
            Region = new RegionSettings();
            Region.LoadSettings();
            Debug.WriteLine("[ FlashLauncherUI:Init ] Loaded Region: " + Region.CurrentSelection);

            switch (Region.CurrentSelection)
            {
                case "eu":
                    ComboBox_RegionSelection.SelectedIndex = 0;
                    break;
                case "us":
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
        private async void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            Progressbar_Login.IsIndeterminate = true;
            if (!String.IsNullOrEmpty(TextBox_Username.Text) || !String.IsNullOrEmpty(PasswordBox_Password.Password))
            {
                Debug.WriteLine("[FlashLauncher:Button_Play_Click] Login start");

                Account account = new Account(username:TextBox_Username.Text,password:PasswordBox_Password.Password);
                API2.SetAccount(account);
                await API2.Login(Region.CurrentSelection);

                Debug.WriteLine("[FlashLauncher:Button_Login_Click] Login done");
                if (!API2.IsLoggedIn)
                {
                    Debug.WriteLine("[FlashLauncher:Button_Play_Click] Was unable to Login");
                }

                if (API2.IsLoggedIn)
                {
                    AccountManager am = new AccountManager();
                    Accounts.Add(account);
                    am.CreateNewAccount(account);

                    // clear data
                    TextBox_Username.Text = "";
                    PasswordBox_Password.Password = ""; 
                }
                else
                {

                }
            }
            Progressbar_Login.IsIndeterminate = false;
        }

        private async void Button_Play_Click(object sender, RoutedEventArgs e)
        {
            Progressbar_Login.IsIndeterminate = true;
            if (Accounts.Count > 0)
            {
                if (!(ListBox_AccountList.SelectedIndex == -1))
                {
                    // set the user account based on the current selected account
                    API2.SetAccount(Accounts[ListBox_AccountList.SelectedIndex]);
                    Debug.WriteLine("[FlashLauncher:Button_Play_Click] Login start");

                    await API2.Login(Region.CurrentSelection);

                    Debug.WriteLine("[FlashLauncher:Button_Play_Click] Login done");
                    if (!API2.IsLoggedIn)
                    {
                        Debug.WriteLine("[FlashLauncher:Button_Play_Click] Was unable to Login");
                    }
                    string launchArgs = await API2.GetLaunchArgs();
                    API2.LaunchGame(launchArgs);
                }
            }
            Progressbar_Login.IsIndeterminate = false;
        }

        private void Button_EditAccount_Click(object sender, RoutedEventArgs e)
        {
            EditAccount();
        }

        private void Button_DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_AccountList.SelectedIndex < 0)
            {
                return;
            }
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
            Debug.WriteLine("[EditAccount] Seleted ID: " + ListBox_AccountList.SelectedIndex.ToString());
            if (ListBox_AccountList.SelectedIndex is not > 0)
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
            BitmapImage ServerRegionImage = new BitmapImage();
            ServerRegionImage.BeginInit();
            switch (ComboBox_RegionSelection.SelectedIndex)
            {
                case 0:
                    Region.CurrentSelection = "EU";
                    ServerRegionImage.UriSource = new Uri("pack://application:,,,/FlashLauncher;component/assets/europaische-union.png");
                    ServerRegionImage.EndInit();
                    Image_ServerRegion.Source = ServerRegionImage;
                    break;
                case 1:
                    Region.CurrentSelection = "US";
                    ServerRegionImage.UriSource = new Uri("pack://application:,,,/FlashLauncher;component/assets/vereinigte-staaten.png");
                    ServerRegionImage.EndInit();
                    Image_ServerRegion.Source = ServerRegionImage;
                    break;
                default:
                    break;
            }
            Region.WriteSettings();
        }
    }
}
