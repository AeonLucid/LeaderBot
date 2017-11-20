using Newtonsoft.Json;

namespace LeaderBot.Config.Games
{
    internal class RocketLeagueConfig : IGameConfig
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        
        [JsonProperty("api_key")]
        public string ApiKey { get; set; } = "";
    }
}