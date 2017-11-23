using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LeaderBot.Games
{
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum Game
    {
        Unknown = 0,
        RocketLeague = 1,
        Overwatch = 2
    }
}
