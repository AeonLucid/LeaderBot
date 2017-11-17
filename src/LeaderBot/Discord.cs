using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using NLog;

namespace LeaderBot
{
    internal class Discord
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly DiscordSocketClient _client;
        
        public Discord()
        {
            _client = new DiscordSocketClient();
            _client.Log += ClientOnLog;
            _client.Connected += ClientOnConnected;
            _client.Disconnected += ClientOnDisconnected;
            _client.GuildAvailable += ClientOnGuildAvailable;
            _client.MessageReceived += ClientOnMessageReceived;
        }

        public async Task StartAsync(string token)
        {
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }
        
        private Task ClientOnLog(LogMessage logMessage)
        {
            const string logFormat = "Discord => '{0}'.";
            
            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                    Logger.Fatal(logFormat, logMessage.Message);
                    break;
                case LogSeverity.Error:
                    Logger.Error(logFormat, logMessage.Message);
                    break;
                case LogSeverity.Warning:
                    Logger.Warn(logFormat, logMessage.Message);
                    break;
                case LogSeverity.Info:
                    Logger.Info(logFormat, logMessage.Message);
                    break;
                case LogSeverity.Verbose:
                    Logger.Debug(logFormat, logMessage.Message);
                    break;
                case LogSeverity.Debug:
                    Logger.Trace(logFormat, logMessage.Message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Task.CompletedTask;
        }

        private Task ClientOnConnected()
        {
            Logger.Info("We are now successfully connected to Discord.");

            return Task.CompletedTask;
        }

        private Task ClientOnDisconnected(Exception exception)
        {
            Logger.Warn(exception, "We somehow got disconnected from Discord.");
            
            // TODO: Do we have to reconnect ourselves ??

            return Task.CompletedTask;
        }

        private Task ClientOnGuildAvailable(SocketGuild guild)
        {
            Logger.Info($"We have entered some weird guild named {guild.Name} and they seem to have {guild.MemberCount} members.");

            return Task.CompletedTask;
        }

        private Task ClientOnMessageReceived(SocketMessage message)
        {
            Logger.Info($"Received a message from '{message.Author.Username}': '{message.Content}'.");

            return Task.CompletedTask;
        }
    }
}