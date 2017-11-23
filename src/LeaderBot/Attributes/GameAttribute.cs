using System;
using LeaderBot.Games;

namespace LeaderBot.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class GameAttribute : Attribute
    {
        public GameAttribute(Game name)
        {
            Name = name;
            DiscordNames = new[] {name.ToString()};
        }

        public GameAttribute(Game name, string[] discordNames)
        {
            Name = name;
            DiscordNames = discordNames;
        }

        public Game Name { get; }

        public string[] DiscordNames { get; }
    }
}