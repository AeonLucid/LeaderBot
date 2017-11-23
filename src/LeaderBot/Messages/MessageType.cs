using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LeaderBot.Messages
{
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum MessageType
    {
        Unknown = 1,
        RocketLeagueSolo = 2,            // 1v1
        RocketLeagueDuo = 3,             // 2v2
        RocketLeagueSoloStandard = 4,    // 3v3 (Everyone alone)
        RocketLeagueStandard = 5,        // 4v4
    }
}
