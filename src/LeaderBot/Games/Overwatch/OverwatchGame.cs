using LeaderBot.Attributes;
using LeaderBot.Games.Base;
using NLog;

namespace LeaderBot.Games.Overwatch
{
    [Game(Game.Overwatch)]
    internal class OverwatchGame : BaseGame<OverwatchConfig>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public OverwatchGame()
        {
        }

        public override OverwatchConfig DefaultConfig => new OverwatchConfig();

        public override void Initialize(OverwatchConfig config)
        {
            Logger.Info("Overwatch loaded.");
        }
    }
}