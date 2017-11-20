using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using LeaderBot.Attributes;
using LeaderBot.Games.Base;

namespace LeaderBot.Services
{
    internal class GameRegisteryService
    {
        public readonly Dictionary<string, BaseGame> Games = new Dictionary<string, BaseGame>();

        public readonly Dictionary<string, string> DiscordGameNames = new Dictionary<string, string>();

        private readonly IComponentContext _context;

        public GameRegisteryService(IComponentContext context)
        {
            _context = context;
        }

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

                    if (!typeof(BaseGame).IsAssignableFrom(type))
                    {
                        throw new Exception("Expected the game to extends BaseGame.");
                    }

                    Games.Add(game.Name, (BaseGame) _context.Resolve(type));

                    foreach (var discordName in game.DiscordNames)
                    {
                        DiscordGameNames.Add(discordName, game.Name);
                    }
                }
            }
        }

        public IEnumerable<string> GetDiscordNames(string gameName)
        {
            return DiscordGameNames.Where(kv => kv.Value.Equals(gameName)).Select(kv => kv.Key);
        }
    }
}