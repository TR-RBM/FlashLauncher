using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FlashLauncher
{
    public class ServerStatusResponse
    {
        [JsonProperty("game_server_status_list")]
        public List<GameServerStatus> GameServerStatusList { get; set; }
    }
}
