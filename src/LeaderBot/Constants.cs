using System.Collections.Generic;
using LeaderBot.Data;

namespace LeaderBot
{
    internal static class Constants
    {
        public static Dictionary<Game, string[]> DiscordGameNames = new Dictionary<Game, string[]>
        {
            {Game.RocketLeague, new []{ "Rocket League" }}
        };
    }
}