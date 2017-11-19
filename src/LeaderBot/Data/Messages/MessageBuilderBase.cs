namespace LeaderBot.Data.Messages
{
    internal abstract class MessageBuilderBase
    {
        protected MessageBuilderBase(Game game)
        {
            Game = game;
        }
        
        public Game Game { get; }
    }
}