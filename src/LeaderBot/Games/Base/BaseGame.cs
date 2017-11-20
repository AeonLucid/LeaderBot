using System;

namespace LeaderBot.Games.Base
{
    internal abstract class BaseGame
    {
        public abstract Type ConfigType { get; }

        public abstract BaseConfig CreateConfig();

        public abstract void Initialize(BaseConfig config);
    }
    
    internal abstract class BaseGame<TC> : BaseGame where TC : BaseConfig
    {
        public override Type ConfigType => typeof(TC);
        
        public abstract TC DefaultConfig { get; }
        
        public override void Initialize(BaseConfig config)
        {
            Initialize((TC) config);
        }

        public virtual void Initialize(TC config)
        {
            
        }

        public override BaseConfig CreateConfig()
        {
            return DefaultConfig;
        }
    }
}