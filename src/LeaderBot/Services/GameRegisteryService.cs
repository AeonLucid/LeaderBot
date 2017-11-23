using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using LeaderBot.Attributes;
using LeaderBot.Games;
using LeaderBot.Games.Base;
using NLog;

namespace LeaderBot.Services
{
    internal class GameRegisteryService
    {
        private readonly IComponentContext _context;

        public GameRegisteryService(IComponentContext context)
        {
            _context = context;

            Games = new Dictionary<Game, IBaseGame>();
            GameNames = new Dictionary<Game, string[]>();
        }

        public Dictionary<Game, IBaseGame> Games { get; }

        public Dictionary<Game, string[]> GameNames { get; }

        public void Load()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var game = type.GetCustomAttribute<GameAttribute>();
                    if (game == null)
                    {
                        continue;
                    }

                    if (!typeof(IBaseGame).IsAssignableFrom(type))
                    {
                        throw new Exception("Expected the game to extend IBaseGame.");
                    }
                    
                    Games.Add(game.Name, (IBaseGame)_context.Resolve(type));
                    GameNames.Add(game.Name, game.DiscordNames);
                }
            }
        }

        public Game GetGame(string game)
        {
            if (string.IsNullOrWhiteSpace(game))
            {
                return Game.Unknown;
            }

            return GameNames.FirstOrDefault(x => x.Value.Contains(game)).Key;
        }

        public IEnumerable<string> GetGameNames(Game game)
        {
            return GameNames[game];
        }
    }
}