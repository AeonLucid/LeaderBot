using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using LeaderBot.Config;
using Newtonsoft.Json;
using NLog;

namespace LeaderBot
{
    internal static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);
        
        private static AppConfig _config;

        private static Discord _discord;
        
        private static void Main(string[] args)
        {
            // Configure the logger.
            LogManager.Configuration = NLogConfig.Create(args.Any(s => s.Equals("--debug")));
            Logger.Info("Preparing for launch.");
            
            // Configure the console.
            Console.Title = "LeaderBot vsomething";
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                QuitEvent.Set();
            };

            // Load path variables.
            var rootDir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            var configDir = Path.Combine(rootDir, "Config");
            var configFile = Path.Combine(configDir, "app_config.json");
            
            // Ensure the config directory is available.
            Directory.CreateDirectory(configDir);
            
            // Ensure a config file is available.
            if (!File.Exists(configFile))
            {
                File.WriteAllText(configFile, JsonConvert.SerializeObject(new AppConfig(), Formatting.Indented));
            }
            
            // Ensure the config is loaded.
            _config = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(configFile));
            
            // Magic.
            
            Logger.Info("Starting LeaderBot.");
            
            InitAsync().GetAwaiter().GetResult();
            
            QuitEvent.WaitOne();

            Logger.Warn("Shutting down.");
            
            // Bye.
        }

        private static async Task InitAsync()
        {
            // Initialize a discord client.
            _discord = new Discord();
            
            // Authenticate and connect to discord.
            await _discord.StartAsync(_config.Discord.Token);
        }
    }
}