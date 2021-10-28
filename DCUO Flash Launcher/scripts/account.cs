using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashLauncher
{
    /// <summary>
    /// Class Object for a user account wich stores the username, password and email
    /// </summary>
    internal class Account
    {
        private string username = new("");
        private string password = new("");
        private string email = new("");

        /// <summary>
        /// Stores the username in Base64 format
        /// </summary>
        internal string Username
        {
            get {
                byte[] bytes = Convert.FromBase64String(username);
                return Encoding.UTF8.GetString(bytes);
            }
            set {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                username = Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// Stores the password in Base64 format
        /// </summary>
        internal string Password
        {
            get
            {
                byte[] bytes = Convert.FromBase64String(password);
                return Encoding.UTF8.GetString(bytes);
            }
            set
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                password = Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// Stores the email in Base64 format
        /// </summary>
        internal string Email
        {
            get
            {
                byte[] bytes = Convert.FromBase64String(email);
                return Encoding.UTF8.GetString(bytes);
            }
            set
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                email = Convert.ToBase64String(bytes);
            }
        }

        internal Account(string username, string password, string email)
        {
            this.Username = username;
            this.Password = password;
            this.Email = email;
        }
    }
}
