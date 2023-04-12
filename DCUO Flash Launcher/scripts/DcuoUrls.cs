using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashLauncher
{
    public class DcuoUrls
    {
        /// <summary>
        /// DCUO Game Launcher
        /// </summary>
        public string Live { get; set; }

        /// <summary>
        /// init Urls
        /// </summary>
        public DcuoUrls()
        {
            Live = "https://lpj.daybreakgames.com/dcuo/live/";
        }
    }
}
