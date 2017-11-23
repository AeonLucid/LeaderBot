using LeaderBot.Attributes;
using LeaderBot.Games.Base;

namespace LeaderBot.Games.Overwatch
{
    [Game(Game.Overwatch)]
    internal class OverwatchGame : BaseGame<OverwatchConfig>
    {
        public override BaseConfig DefaultConfig => new OverwatchConfig();
    }
}