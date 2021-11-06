using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

namespace FlashLauncher
{
    /// <summary>
    /// Class Object for managing the accounts in the local sqlite database
    /// </summary>
    public class AccountManager
    {
        /// <summary>
        /// list that stores the accounts while reading and writeing to the database
        /// </summary>
        public List<Account> accounts = new();
        private string databasePath = new string("accounts.sqlite");

        public AccountManager()
        {            
            // check if the database works and if the table exists
            // if thats not the case, create the table
            using (SqliteConnection connection = new("Data Source=" + databasePath))
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "CREATE TABLE IF NOT EXISTS accounts(id INTEGER PRIMARY KEY AUTOINCREMENT, username TEXT, password TEXT)";
                command.ExecuteNonQuery();
                connection.Close();
            }

        }

        /// <summary>
        /// Adds new account to the database
        /// </summary>
        /// <param name="account"></param>
        public void CreateNewAccount(Account account)
        {
            accounts.Add(account);
            SaveToDatabase();
        }

        public void SaveToDatabase()
        {
            try
            {
                using (SqliteConnection connection = new ("Data Source=" + databasePath))
                {
                    connection.Open();
                    foreach (Account account in accounts)
                    {
                        SqliteCommand command = connection.CreateCommand();
                        command.CommandText = "INSERT INTO accounts (username, password) VALUES (\""+ account.username +"\",\""+ account.password +"\");";
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Returns a list of all accounts in the local database
        /// can return null, if there is no database
        /// </summary>
        public void LoadFromDatabase()
        {
            if (File.Exists(databasePath))
            {
                try
                {
                    using (SqliteConnection connection = new("Data Source=" + databasePath))
                    {
                        connection.Open();
                        SqliteCommand command = connection.CreateCommand();
                        command.CommandText = "SELECT username, password FROM accounts;";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string inputUsername = reader.GetString(0);
                                string inputPassword = reader.GetString(1);

                                if (!String.IsNullOrWhiteSpace(inputUsername) && !String.IsNullOrWhiteSpace(inputPassword))
                                {
                                    accounts.Add(new Account()
                                    {
                                        username = inputUsername,
                                        password = inputPassword
                                    });
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
