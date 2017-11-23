using System.Collections.Generic;
using LeaderBot.Attributes;
using LeaderBot.Config;
using LeaderBot.Games.Base;
using LeaderBot.Messages;
using RLSApi.Data;

namespace LeaderBot.Games.RocketLeague
{
    [Game(Game.RocketLeague, new[] {"Rocket League"})]
    internal class RocketLeagueGame : BaseGame<RocketLeagueConfig>
    {
        public override BaseConfig DefaultConfig => new RocketLeagueConfig
        {
            ApiKey = "Sign up at https://developers.rocketleaguestats.com/",
            Channel = new ChannelConfig
            {
                Name = "rocket-league",
                Messages = new List<MessageType>
                {
                    MessageType.RocketLeagueSolo,
                    MessageType.RocketLeagueDuo,
                    MessageType.RocketLeagueSoloStandard,
                    MessageType.RocketLeagueStandard
                }
            },
            Players = new List<RocketLeaguePlayer>
            {
                new RocketLeaguePlayer
                {
                    UniqueId = "76561198033338223",
                    Platform = RlsPlatform.Steam
                }
            }
        };
    }
}