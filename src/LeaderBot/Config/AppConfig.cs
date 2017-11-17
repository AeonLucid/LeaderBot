using Newtonsoft.Json;

namespace LeaderBot.Config
{
    internal class AppConfig
    {
        [JsonProperty("discord", Required = Required.Always)]
        public DiscordConfig Discord { get; set; } = new DiscordConfig();
    }
}