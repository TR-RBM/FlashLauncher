using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel;

using static System.Net.WebRequestMethods;

namespace FlashLauncher
{
    public static class URL
    {
        /// <summary>
        /// DCUO Game Launcher
        /// </summary>
        public static string Live = "https://lpj.daybreakgames.com/dcuo/live/";

        /// <summary>
        /// URL for getting the launch arguments for the game
        /// </summary>
        public static string GetPlaySession = "https://lpj.daybreakgames.com/dcuo/live/get_play_session";

        /// <summary>
        /// URL for Login Process
        /// </summary>
        public static string Login = "https://lpj.daybreakgames.com/dcuo/live/login";
    }
}
