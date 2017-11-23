using System;
using NLog;

namespace LeaderBot.Games.Base
{
    internal abstract class BaseGame<TC> : IBaseGame where TC : BaseConfig
    {
        public static Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     The configuration <see cref="Type"/> of <see cref="TC"/>.
        /// </summary>
        public Type ConfigType => typeof(TC);

        /// <summary>
        ///     The base configuration when using the <see cref="IBaseGame"/> instance.
        /// </summary>
        public BaseConfig LoadedConfigBase { get; set; }

        /// <summary>
        ///     The full configuration when using the <see cref="BaseGame{TC}"/> instance.
        /// </summary>
        public TC LoadedConfig => (TC) LoadedConfigBase;

        /// <summary>
        ///     The default configuration deployed on first time use.
        /// </summary>
        public abstract BaseConfig DefaultConfig { get; }

        /// <summary>
        ///     Loads the configuration.
        /// </summary>
        /// <param name="gameConfig"></param>
        public void Load(BaseConfig gameConfig)
        {
            if (ConfigType != gameConfig.GetType())
            {
                throw new ArgumentException(nameof(gameConfig));
            }

            LoadedConfigBase = gameConfig;

            Logger.Trace($"Loaded the {ConfigType.Name}.");
        }

        /// <summary>
        ///     The game "plugin" is ready to be used by LeaderBot and
        ///     was loaded properly.
        /// </summary>
        public virtual void Initialize()
        {
            
        }
    }
}