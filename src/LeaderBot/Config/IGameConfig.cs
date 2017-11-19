using LeaderBot.Data;

namespace LeaderBot.Config
{
    internal interface IGameConfig
    {
        Game Game { get; }
        
        bool Enabled { get; set; }
    }
}