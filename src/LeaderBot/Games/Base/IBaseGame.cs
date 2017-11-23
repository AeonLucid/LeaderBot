using System;

namespace LeaderBot.Games.Base
{
    internal interface IBaseGame
    {
        Type ConfigType { get; }

        BaseConfig LoadedConfigBase { get; set; }

        BaseConfig DefaultConfig { get; }

        void Load(BaseConfig gameConfig);

        void Initialize();
    }
}
