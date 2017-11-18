using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.NLog;
using LeaderBot.Config;
using LeaderBot.Services;
using NLog;
using ILogger = Autofac.Extras.NLog.ILogger;

namespace LeaderBot
{
    internal static class Program
    {
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);
        
        private static async Task Main(string[] args)
        {
            // Configure the logger.
            LogManager.Configuration = NLogConfig.Create(args.Any(s => s.Equals("--debug")));
            
            // Configure the console.
            Console.Title = "LeaderBot vsomething";
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                QuitEvent.Set();
            };
            
            // Configure DI.
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule<SimpleNLogModule>();
            
            containerBuilder.RegisterType<DiscordService>()
                .AsSelf()
                .SingleInstance();

            containerBuilder.RegisterType<ConfigProviderService<AppConfig>>()
                .AsSelf()
                .SingleInstance()
                .OnActivating(async eventArgs => await eventArgs.Instance.LoadAsync());;
            
            // Start the program.
            using (var container = containerBuilder.Build())
            {
                var logger = container.Resolve<ILogger>();
                var discordService = container.Resolve<DiscordService>();

                logger.Info("Starting LeaderBot.");
                
                await discordService.StartAsync();
                
                QuitEvent.WaitOne();
                
                logger.Warn("Shutting down.");
            }
            
            // Bye.
        }
    }
}