using System;
using Autofac.Extras.NLog;
using LeaderBot.Attributes;
using LeaderBot.Games.Base;

namespace LeaderBot.Games.Overwatch
{
    [Game("Overwatch")]
    internal class OverwatchGame : BaseGame<OverwatchConfig>
    {
        private readonly ILogger _logger;

        public OverwatchGame(ILogger logger)
        {
            _logger = logger;
        }

        public override void Initialize(OverwatchConfig config)
        {
            _logger.Info("Overwatch loaded.");
        }

        public override OverwatchConfig DefaultConfig => new OverwatchConfig();
    }
}