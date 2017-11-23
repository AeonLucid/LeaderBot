using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Net.WebSocket;
using LeaderBot.Config;
using LeaderBot.Games;
using NLog;
using LogLevel = DSharpPlus.LogLevel;

namespace LeaderBot.Services
{
    internal class DiscordService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly AppConfig _config;

        private readonly DiscordClient _client;

        private readonly GameRegisteryService _gameRegistery;

        // TODO: Only count players that are registered for that specific game leaderboard.
        private readonly Dictionary<Game, int> _playerAmount;

        public DiscordService(ConfigProviderService<AppConfig> configProvider, GameRegisteryService gameRegistery)
        {
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
            _playerAmount = _gameRegistery.Games.ToDictionary(x => x.Key, y => 0);
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
            Logger.Info("We are now ready to do stuff.");

            return Task.CompletedTask;
        }

        private async Task ClientOnGuildAvailable(GuildCreateEventArgs e)
        {
            var guild = e.Guild;

            Logger.Debug($"We have entered some weird guild named {guild.Name} and they seem to have {guild.MemberCount} members.");

            if (guild.Id == _config.Discord.GuildId)
            {
                Logger.Info($"Successfully joined the target guild called {guild.Name}.");
                
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

            var gameBefore = e.PresenceBefore?.Game?.Name;
            var gameNow = e.Game?.Name;
            
            // Check if the game even changed.
            if (gameNow != gameBefore || 
                gameBefore != null && gameNow != null && gameBefore.ToLower().Equals(gameNow.ToLower()))
            {
                return Task.CompletedTask;
            }

            Logger.Trace($"{e.Member.DisplayName} is now playing {gameNow ?? "{Nothing}"} instead of {gameBefore ?? "{Nothing}"}");

            UpdatePlayerCount(gameBefore, gameNow);
            
            return Task.CompletedTask;
        }

        private Task ClientOnClientErrored(ClientErrorEventArgs e)
        {
            Logger.Error(e.Exception, "We are broke.");

            return Task.CompletedTask;
        }

        private void DebugLoggerOnLogMessageReceived(object sender, DebugLogMessageEventArgs e)
        {
            const string logFormat = "Discord => '{0}'.";
            
            switch (e.Level)
            {
                case LogLevel.Debug:
                    Logger.Debug(logFormat, e.Message);
                    break;
                case LogLevel.Info:
                    Logger.Info(logFormat, e.Message);
                    break;
                case LogLevel.Warning:
                    Logger.Warn(logFormat, e.Message);
                    break;
                case LogLevel.Error:
                    Logger.Error(logFormat, e.Message);
                    break;
                case LogLevel.Critical:
                    Logger.Fatal(logFormat, e.Message);
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
            var previousGame = _gameRegistery.GetGame(previousGameStr);
            if (previousGame != Game.Unknown)
            {
                _playerAmount[previousGame]--;
            }

            var nextGame = _gameRegistery.GetGame(nextGameStr);
            if (nextGame != Game.Unknown)
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
            Logger.Trace("Initializing the target guild.");
            
            var channels = await Guild.GetChannelsAsync();

            // Get or create a category channel for the leaderboard(s).
            var categoryChannel = channels.FirstOrDefault(x => x.IsCategory && x.Name.Equals(_config.Discord.CategoryName));
            if (categoryChannel == null)
            {
                Logger.Trace("Creating the category channel for the leaderboard(s).");

                categoryChannel = await Guild.CreateChannelAsync(_config.Discord.CategoryName, ChannelType.Category);
            }
            
            // Update the playing count.
            var members = await Guild.GetAllMembersAsync();

            foreach (var member in members)
            {
                UpdatePlayerCount(string.Empty, member.Presence?.Game?.Name);
            }

            var playingArray = _playerAmount.Where(kv => kv.Value > 0).Select(kv => $"{kv.Key}: {kv.Value}").ToArray();
            var playingString = playingArray.Length > 0 ? string.Join(", ", playingArray) : "absolutly nothing";
            Logger.Trace($"Currently the guild is playing: {playingString}");
            
            // TODO: Create the channels.
        }
     }  
}