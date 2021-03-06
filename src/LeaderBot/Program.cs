﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using LeaderBot.Config;
using LeaderBot.Config.Converter;
using LeaderBot.Games.Base;
using LeaderBot.Services;
using Newtonsoft.Json;
using NLog;

namespace LeaderBot
{
    internal static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
            
            containerBuilder.RegisterType<JsonSerializerSettings>()
                .AsSelf()
                .SingleInstance()
                .OnActivating(eventArgs =>
                {
                    eventArgs.Instance.Converters = new List<JsonConverter>
                    {
                        new GameJsonConverter(eventArgs.Context.Resolve<GameRegisteryService>())
                    };
                });
            
            containerBuilder.RegisterType<GameRegisteryService>()
                .AsSelf()
                .SingleInstance()
                .OnActivating(eventArgs => eventArgs.Instance.Load());
            
            containerBuilder.RegisterType<DiscordService>()
                .AsSelf()
                .SingleInstance();

            containerBuilder.RegisterType<ConfigProviderService<AppConfig>>()
                .AsSelf()
                .SingleInstance()
                .OnActivating(async eventArgs => await eventArgs.Instance.LoadAsync());

            // Register all the games from all the loaded assemblies.
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                containerBuilder.RegisterAssemblyTypes(assembly)
                    .AssignableTo<IBaseGame>()
                    .AsSelf()
                    .SingleInstance();
            }
            
            // Start the program.
            using (var container = containerBuilder.Build())
            {
                var discordService = container.Resolve<DiscordService>();
                var configProvider = container.Resolve<ConfigProviderService<AppConfig>>();
                var gameRegistery = container.Resolve<GameRegisteryService>();
                
                // Prepare the config.
                configProvider.Config.Prepare(gameRegistery.Games.Values);

                await configProvider.SaveAsync();

                // Launch the application.
                Logger.Info("Starting LeaderBot.");
                
                await discordService.StartAsync();
                
                // Load the games.
                foreach (var (game, gameBase) in gameRegistery.Games)
                {
                    var gameConfig = configProvider.Config.Games.FirstOrDefault(x => x.GetType() == gameBase.ConfigType);
                    
                    if (gameConfig == null)
                    {
                        throw new Exception($"The game {gameBase} does not have a config loaded.");
                    }

                    if (!gameConfig.Enabled)
                    {
                        continue;
                    }
                
                    if (!gameRegistery.GetGameNames(game).Any())
                    {
                        Logger.Warn($"The game {game} does not have any Discord names.");
                    }

                    Logger.Trace($"Enabling the game {game}.");

                    gameBase.Load(gameConfig);
                    gameBase.Initialize();
                }
                
                QuitEvent.WaitOne();

                Logger.Warn("Shutting down.");
            }
            
            // Bye.
            LogManager.Shutdown();
        }
    }
}