using Newtonsoft.Json;

namespace LeaderBot.Config
{
    internal class AppConfig
    {
        [JsonProperty("discord")]
        public DiscordConfig Discord { get; set; } = new DiscordConfig();
    }
}