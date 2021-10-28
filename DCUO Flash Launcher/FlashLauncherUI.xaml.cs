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
        }

        private void Button_AddAccount_Click(object sender, RoutedEventArgs e)
        {
            Account account = new("Tim", "p4ssw0rd", "info@tr-rbm.de");
            Accounts.Add(account);
        }
    }
}
