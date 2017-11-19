using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Net.WebSocket;
using LeaderBot.Config;
using Newtonsoft.Json;

namespace LeaderBot.Services
{
    internal class DiscordService
    {
        private readonly ILogger _logger;

        private readonly AppConfig _config;

        private readonly DiscordClient _client;

        public DiscordService(ILogger logger, ConfigProviderService<AppConfig> configProvider)
        {
            _logger = logger;
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

        private Task ClientOnClientErrored(ClientErrorEventArgs clientErrorEventArgs)
        {
            _logger.Error("We are broke.");

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
        }
     }  
}