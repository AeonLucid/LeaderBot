using Newtonsoft.Json;

namespace LeaderBot.Config
{
    internal class DiscordConfig
    {
        [JsonProperty("token", Required = Required.Always)]
        public string Token { get; set; } = "";
    }
}