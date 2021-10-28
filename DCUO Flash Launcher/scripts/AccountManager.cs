using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FlashLauncher
{
    /// <summary>
    /// Class Object for managing the accounts in the local sqlite database
    /// </summary>
    internal class AccountManager
    {
        /// <summary>
        /// list that stores the accounts while reading and writeing to the database
        /// </summary>
        private List<Account> accounts = new();

        internal AccountManager()
        {
        }

        /// <summary>
        /// Returns a list of all accounts in the local database
        /// </summary>
        /// <returns>Accounts[]</returns>
        internal Account[] GetAccountList()
        {
            return new Account[] {};
        }
    }
}
