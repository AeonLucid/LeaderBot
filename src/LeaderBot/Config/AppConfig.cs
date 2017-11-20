using System.Collections.Generic;
using System.Linq;
using LeaderBot.Games.Base;
using Newtonsoft.Json;

namespace LeaderBot.Config
{
    internal class AppConfig
    {
        [JsonProperty("discord")]
        public DiscordConfig Discord { get; set; } = new DiscordConfig();
        
        [JsonProperty("games")]
        public List<BaseConfig> Games { get; set; } = new List<BaseConfig>();

        public void Prepare(IEnumerable<BaseGame> games)
        {
            foreach (var game in games)
            {
                if (Games.All(x => x.GetType() != game.ConfigType))
                {
                    Games.Add(game.CreateConfig());
                }
            }
        }
    }
}