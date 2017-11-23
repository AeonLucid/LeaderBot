using LeaderBot.Config;
using Newtonsoft.Json;

namespace LeaderBot.Games.Base
{
    internal abstract class BaseConfig
    {
        protected BaseConfig(Game game)
        {
            Game = game;
        }

        [JsonProperty("game")]
        public Game Game { get; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("channel")]
        public ChannelConfig Channel { get; set; }
    }
}