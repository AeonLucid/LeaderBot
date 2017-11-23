using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RLSApi.Data;

namespace LeaderBot.Games.RocketLeague
{
    internal class RocketLeaguePlayer
    {
        /// <summary>
        ///     Steam: Steam64 id, ex; "76561198033338223".
        ///     Ps4: psn name.
        ///     Xbox: gamertag or xuid.
        /// </summary>
        [JsonProperty("unique_id")]
        public string UniqueId { get; set; }

        /// <summary>
        ///     Can be Steam, Ps4 and Xbox.
        /// </summary>
        [JsonProperty("platform")]
        [JsonConverter(typeof(StringEnumConverter))]
        public RlsPlatform Platform { get; set; }
    }
}