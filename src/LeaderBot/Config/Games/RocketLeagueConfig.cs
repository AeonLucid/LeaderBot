using LeaderBot.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LeaderBot.Config.Games
{
    internal class RocketLeagueConfig : IGameConfig
    {
        [JsonProperty("game")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Game Game { get; } = Game.RocketLeague;
        
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        
        [JsonProperty("api_key")]
        public string ApiKey { get; set; } = "";
    }
}