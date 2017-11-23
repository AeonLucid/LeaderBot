using System.Collections.Generic;
using LeaderBot.Games.Base;
using Newtonsoft.Json;

namespace LeaderBot.Games.RocketLeague
{
    internal class RocketLeagueConfig : BaseConfig
    {
        public RocketLeagueConfig() : base(Game.RocketLeague)
        {
        }

        [JsonProperty("api_key")]
        public string ApiKey { get; set; }

        [JsonProperty("players")]
        public List<RocketLeaguePlayer> Players { get; set; }
    }
}