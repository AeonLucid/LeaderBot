using System;
using LeaderBot.Games;
using LeaderBot.Games.Base;
using LeaderBot.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LeaderBot.Config.Converter
{
    internal class GameJsonConverter : JsonConverter
    {
        private readonly GameRegisteryService _gameRegistery;

        public GameJsonConverter(GameRegisteryService gameRegistery)
        {
            _gameRegistery = gameRegistery;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var token = (JObject)JToken.FromObject(value);

            token.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var game = token["game"].ToObject<Game>();
            
            if (!_gameRegistery.Games.TryGetValue(game, out var baseGame))
            {
                throw new JsonReaderException($"The game {game.ToString()} is invalid.");
            }

            return token.ToObject(baseGame.ConfigType);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(BaseConfig).IsAssignableFrom(objectType);
        }

        public override bool CanRead => true;
    }
}