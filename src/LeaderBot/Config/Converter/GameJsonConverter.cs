using System;
using System.Linq;
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
            var gameName = _gameRegistery.Games.First(kv => kv.Value.ConfigType == value.GetType()).Key;
            
            token.AddFirst(new JProperty("game", gameName));
            token.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var gameName = (string)token["game"];
            
            if (!_gameRegistery.Games.TryGetValue(gameName, out var game))
            {
                throw new JsonReaderException($"The game {gameName} is invalid.");
            }

            return token.ToObject(game.ConfigType);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(BaseConfig).IsAssignableFrom(objectType);
        }

        public override bool CanRead => true;
    }
}