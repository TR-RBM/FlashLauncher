using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashLauncher
{
    public class ServerStatusViewModel
    {
        /// <summary>
        /// Name of the Server
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Server Status String
        /// </summary>
        public string ServerStatus { get; set; }

        /// <summary>
        /// Path to the Status image
        /// </summary>
        public string StatusIcon { get; set; } 
    }
}
