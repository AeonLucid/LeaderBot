using System;
using System.Collections.Generic;
using LeaderBot.Config.Games;
using LeaderBot.Data;

namespace LeaderBot
{
    internal static class Constants
    {
        public static readonly Dictionary<string, Game> DiscordGameNames = new Dictionary<string, Game>
        {
            {"Rocket League", Game.RocketLeague}
        };
        
        public static readonly Dictionary<Game, Type> ConfigTypes = new Dictionary<Game, Type>
        {
            {Game.RocketLeague, typeof(RocketLeagueConfig)}
        };
    }
}