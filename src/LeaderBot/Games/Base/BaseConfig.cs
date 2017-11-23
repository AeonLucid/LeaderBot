using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LeaderBot.Games.Base
{
    internal abstract class BaseConfig
    {
        [JsonProperty("game")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Game Game { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }
}