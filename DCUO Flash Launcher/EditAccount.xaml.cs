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

namespace FlashLauncher
{
    /// <summary>
    /// Interaktionslogik für EditAccount.xaml
    /// </summary>
    public partial class EditAccount : Window
    {
        public string username = new("");
        public string password = new("");
        public string Username { get; set; }
        public string Password {  get; set; }

        public EditAccount(string username, string password)
        {
            InitializeComponent();
            TextBox_Username.Text = username;
            PasswordBox_Password.Text = password;
            this.username = username;
            this.password = password;
            Title = "FlashLauncher > EditAccount > " + username;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            username = TextBox_Username.Text;
            password = PasswordBox_Password.Text;
            this.Close();
        }
    }
}
