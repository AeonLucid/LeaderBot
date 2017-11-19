using Newtonsoft.Json;

namespace LeaderBot.Config
{
    internal class DiscordConfig
    {
        [JsonProperty("token")]
        public string Token { get; set; } = "";

        [JsonProperty("category_name")]
        public string CategoryName { get; set; } = "Leaderboards";
    }
}