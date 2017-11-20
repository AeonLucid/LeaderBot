using Newtonsoft.Json;

namespace LeaderBot.Games.Base
{
    internal abstract class BaseConfig
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }
}