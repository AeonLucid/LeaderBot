using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Net.WebSocket;
using LeaderBot.Config;

namespace LeaderBot.Services
{
    internal class DiscordService
    {
        private readonly ILogger _logger;

        private readonly AppConfig _config;

        private readonly DiscordClient _client;

        private readonly GameRegisteryService _gameRegistery;

        // TODO: Only count players that are registered for that specific game leaderboard.
        
        // TODO: Count players at initialize, either grab all users or every user by his discord id.
        //    Preferably the last one as we have to store the discord user ids anyways.
        private readonly Dictionary<string, int> _playerAmount;

        public DiscordService(ILogger logger, ConfigProviderService<AppConfig> configProvider, GameRegisteryService gameRegistery)
        {
            _logger = logger;
            _gameRegistery = gameRegistery;
            
            _config = configProvider.Config;
            
            _client = new DiscordClient(new DiscordConfiguration
            {
                AutoReconnect = true,
                Token = _config.Discord.Token,
                TokenType = TokenType.Bot,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = false
            });
            
            _client.SetWebSocketClient<WebSocket4NetCoreClient>();
            _client.Ready += ClientOnReady;
            _client.PresenceUpdated += ClientOnPresenceUpdated;
            _client.GuildAvailable += ClientOnGuildAvailable;
            _client.ClientErrored += ClientOnClientErrored;
            _client.DebugLogger.LogMessageReceived += DebugLoggerOnLogMessageReceived;
        }

        /// <summary>
        ///     The guild containing the leaderboards.
        /// </summary>
        public DiscordGuild Guild { get; private set; }
        
        public async Task StartAsync()
        {
            await _client.ConnectAsync();
        }

        private Task ClientOnReady(ReadyEventArgs readyEventArgs)
        {
            _logger.Info("We are now ready to do stuff.");

            return Task.CompletedTask;
        }

        private async Task ClientOnGuildAvailable(GuildCreateEventArgs e)
        {
            var guild = e.Guild;
            
            _logger.Debug($"We have entered some weird guild named {guild.Name} and they seem to have {guild.MemberCount} members.");

            if (guild.Id == _config.Discord.GuildId)
            {
                _logger.Info($"Successfully joined the target guild called {guild.Name}.");
                
                Guild = guild;
                
                await InitializeGuild();
            }
        }
        
        private Task ClientOnPresenceUpdated(PresenceUpdateEventArgs e)
        {
            if (Guild == null || e.Guild.Id != Guild.Id)
            {
                return Task.CompletedTask;
            }

            // Check if the game even changed.
            if (e.PresenceBefore.Game.Name.ToLower().Equals(e.Game.Name.ToLower()))
            {
                return Task.CompletedTask;
            }
            
            _logger.Trace($"{e.Member.DisplayName} is now playing {e.Game.Name} instead of {e.PresenceBefore.Game.Name}");

            UpdatePlayerCount(e.PresenceBefore.Game.Name, e.Game.Name);
            
            return Task.CompletedTask;
        }

        private Task ClientOnClientErrored(ClientErrorEventArgs e)
        {
            _logger.Error("We are broke.", e.Exception);

            return Task.CompletedTask;
        }

        private void DebugLoggerOnLogMessageReceived(object sender, DebugLogMessageEventArgs e)
        {
            const string logFormat = "Discord => '{0}'.";
            
            switch (e.Level)
            {
                case LogLevel.Debug:
                    _logger.Debug(logFormat, e.Message);
                    break;
                case LogLevel.Info:
                    _logger.Info(logFormat, e.Message);
                    break;
                case LogLevel.Warning:
                    _logger.Warn(logFormat, e.Message);
                    break;
                case LogLevel.Error:
                    _logger.Error(logFormat, e.Message);
                    break;
                case LogLevel.Critical:
                    _logger.Fatal(logFormat, e.Message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Holds track of the amount of players for each
        ///     game in the <see cref="_playerAmount"/> dictionary.
        /// </summary>
        /// <param name="previousGameStr"></param>
        /// <param name="nextGameStr"></param>
        private void UpdatePlayerCount(string previousGameStr, string nextGameStr)
        {
            if (!string.IsNullOrWhiteSpace(previousGameStr) &&
                _gameRegistery.DiscordGameNames.TryGetValue(previousGameStr, out var previousGame))
            {
                _playerAmount[previousGame]--;
            }

            if (!string.IsNullOrWhiteSpace(nextGameStr) &&
                _gameRegistery.DiscordGameNames.TryGetValue(nextGameStr, out var nextGame))
            {
                _playerAmount[nextGame]++;
            }
        }
        
        /// <summary>
        ///     Makes sure that a guild satisfies the requirements
        ///     to be used for this bot.
        /// </summary>
        /// <returns></returns>
        private async Task InitializeGuild()
        {
            _logger.Trace($"Initializing the target guild.");
            
            var channels = await Guild.GetChannelsAsync();

            // Get or create a category channel for the leaderboard(s).
            var categoryChannel = channels.FirstOrDefault(x => x.IsCategory && x.Name.Equals(_config.Discord.CategoryName));
            if (categoryChannel == null)
            {
                _logger.Trace($"Creating the category channel for the leaderboard(s).");
                
                await Guild.CreateChannelAsync(_config.Discord.CategoryName, ChannelType.Category);
            }
            
            // Create leaderboard channels.
            foreach (var (gameName, game) in _gameRegistery.Games)
            {
                var configType = game.ConfigType;
                
                var gameConfig = _config.Games.FirstOrDefault(x => x.GetType() == configType);
                if (gameConfig == null)
                {
                    throw new Exception($"The game {game} does not have a config loaded.");
                }

                if (!gameConfig.Enabled)
                {
                    continue;
                }
                
                var gameNames = _gameRegistery.GetDiscordNames(gameName).ToArray();
                if (gameNames.Length == 0)
                {
                    throw new Exception($"The game {game} does not have a name defined in the constants.");
                }
            }
            
            _logger.Trace($"Finished initializing the target guild.");
        }
     }  
}