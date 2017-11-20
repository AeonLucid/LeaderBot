using LeaderBot.Games.Base;
using Newtonsoft.Json;

namespace LeaderBot.Games.RocketLeague
{
    internal class RocketLeagueConfig : BaseConfig
    {
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }
    }
}