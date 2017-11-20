using LeaderBot.Attributes;
using LeaderBot.Games.Base;

namespace LeaderBot.Games.RocketLeague
{
    [Game("RocketLeague", DiscordNames = new[] {"Rocket League"})]
    internal class RocketLeagueGame : BaseGame<RocketLeagueConfig>
    {
        public override RocketLeagueConfig DefaultConfig => new RocketLeagueConfig
        {
            ApiKey = "change_me"
        };
    }
}