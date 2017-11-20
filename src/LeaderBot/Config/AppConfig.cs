using System.Collections.Generic;
using System.Linq;
using LeaderBot.Config.Games;
using Newtonsoft.Json;

namespace LeaderBot.Config
{
    internal class AppConfig
    {
        [JsonProperty("discord")]
        public DiscordConfig Discord { get; set; } = new DiscordConfig();
        
        [JsonProperty("games")]
        public List<IGameConfig> Games { get; set; } = new List<IGameConfig>();

        public void Prepare()
        {
            if (Games.All(x => x.GetType() != typeof(RocketLeagueConfig)))
            {
                Games.Add(new RocketLeagueConfig());
            }
        }
    }
}