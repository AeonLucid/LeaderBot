using System;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Discord;
using Discord.WebSocket;
using LeaderBot.Config;

namespace LeaderBot.Services
{
    internal class DiscordService
    {
        private readonly ILogger _logger;

        private readonly AppConfig _config;

        private readonly DiscordSocketClient _client;

        public DiscordService(ILogger logger, ConfigProviderService<AppConfig> configProvider)
        {
            _logger = logger;
            _config = configProvider.Config;
            _client = new DiscordSocketClient();
            _client.Log += ClientOnLog;
            _client.Connected += ClientOnConnected;
            _client.Ready += ClientOnReady;
            _client.Disconnected += ClientOnDisconnected;
            _client.GuildAvailable += ClientOnGuildAvailable;
            _client.MessageReceived += ClientOnMessageReceived;
        }
        
        public bool Connected { get; private set; }
        
        public bool Ready { get; private set; }

        public async Task StartAsync()
        {
            await _client.LoginAsync(TokenType.Bot, _config.Discord.Token);
            await _client.StartAsync();
        }
        
        private Task ClientOnLog(LogMessage logMessage)
        {
            const string logFormat = "Discord => '{0}'.";
            
            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                    _logger.Fatal(logFormat, logMessage.Message);
                    break;
                case LogSeverity.Error:
                    _logger.Error(logFormat, logMessage.Message);
                    break;
                case LogSeverity.Warning:
                    _logger.Warn(logFormat, logMessage.Message);
                    break;
                case LogSeverity.Info:
                    _logger.Info(logFormat, logMessage.Message);
                    break;
                case LogSeverity.Verbose:
                    _logger.Debug(logFormat, logMessage.Message);
                    break;
                case LogSeverity.Debug:
                    _logger.Trace(logFormat, logMessage.Message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Task.CompletedTask;
        }

        private Task ClientOnConnected()
        {
            Connected = true;
            
            _logger.Info("We are now successfully connected to Discord.");

            return Task.CompletedTask;
        }

        private Task ClientOnReady()
        {
            Ready = true;
            
            _logger.Info("We are now ready to do stuff.");

            return Task.CompletedTask;
        }

        private Task ClientOnDisconnected(Exception exception)
        {
            Connected = false;
            
            _logger.Warn("We somehow got disconnected from Discord.", exception);

            return Task.CompletedTask;
        }

        private Task ClientOnGuildAvailable(SocketGuild guild)
        {
            _logger.Info($"We have entered some weird guild named {guild.Name} and they seem to have {guild.MemberCount} members.");

            return Task.CompletedTask;
        }

        private Task ClientOnMessageReceived(SocketMessage message)
        {
            _logger.Info($"Received a message from '{message.Author.Username}': '{message.Content}'.");

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Makes sure that a guild satisfies the requirements
        ///     to be used for this bot.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        private async Task InitializeGuild(SocketGuild guild)
        {
            
        }
     }  
}