using System;

namespace LeaderBot.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GameAttribute : Attribute
    {
        public readonly string Name;
        
        public string[] DiscordNames;

        public GameAttribute(string name)
        {
            Name = name;
            DiscordNames = new[] {name};
        }
    }
}