using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using LeaderBot.Config.Games;
using LeaderBot.Data;
using Newtonsoft.Json;

namespace LeaderBot.Config
{
    internal class AppConfig
    {
        [JsonProperty("discord")]
        public DiscordConfig Discord { get; set; } = new DiscordConfig();
        
        [JsonProperty("games")]
        public List<IGameConfig> Games { get; set; } = new List<IGameConfig>();
    }
}