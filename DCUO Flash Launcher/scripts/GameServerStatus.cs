using Newtonsoft.Json;

namespace FlashLauncher
{
    public class GameServerStatus
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("game_code")]
        public string GameCode { get; set; }

        [JsonProperty("region_name")]
        public string RegionName { get; set; }

        [JsonProperty("last_reported_state")]
        public string LastReportedState { get; set; }

        [JsonProperty("last_reported_time")]
        public long LastReportedTime { get; set; }
    }
}
