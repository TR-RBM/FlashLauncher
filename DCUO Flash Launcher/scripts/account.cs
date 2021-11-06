using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashLauncher
{
    /// <summary>
    /// Class Object for a user account wich stores the username, password and email
    /// </summary>
    public class Account
    {
        /// <summary>
        /// username stored in base64 format
        /// </summary>
        public string username = new("");
        /// <summary>
        /// password stored in base64 format
        /// </summary>
        public string password = new("");

        /// <summary>
        ///  Gets clear text username. Sets username in Base64 format.
        /// </summary>
        public string Username
        {
            get {
                byte[] bytes;
                string utf8username;
                try
                {
                    bytes = Convert.FromBase64String(username);
                    utf8username = Encoding.UTF8.GetString(bytes);
                }
                catch (FormatException e)
                {
                    Debug.WriteLine(e.ToString());
                    return username;
                }
                return utf8username;
            }
            set {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                username = Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// Gets clear text password. Sets password in Base64 format
        /// </summary>
        public string Password
        {
            get
            {
                byte[] bytes;
                string utf8password;
                try
                {
                    bytes = Convert.FromBase64String(password);
                    utf8password = Encoding.UTF8.GetString(bytes);
                }
                catch (FormatException e)
                {
                    Debug.WriteLine(e.ToString());
                    return password;
                }
                return utf8password;
            }
            set
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                password = Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// Create new Account with username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public Account(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        /// <summary>
        /// Creates new Account with no username or password
        /// </summary>
        public Account()
        {
        }
    }
}