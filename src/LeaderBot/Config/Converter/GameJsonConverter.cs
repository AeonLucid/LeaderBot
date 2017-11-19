using System;
using System.Collections.Generic;
using System.Linq;
using LeaderBot.Config.Games;
using LeaderBot.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LeaderBot.Config.Converter
{
    public class GameJsonConverter : JsonConverter
    {
        private readonly Dictionary<Game, Type> _gameTypes = new Dictionary<Game, Type>
        {
            {Game.RocketLeague, typeof(RocketLeagueConfig)}
        };
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var token = (JObject)JToken.FromObject(value);
            var game = _gameTypes.First(kv => kv.Value == value.GetType()).Key;
            
            token.AddFirst(new JProperty("game", game.ToString()));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var gameName = (string)token["game"];

            if (!Enum.TryParse<Game>(gameName, out var game))
            {
                throw new JsonReaderException($"Unknown game '{gameName}'.");
            }

            if (!_gameTypes.TryGetValue(game, out var type))
            {
                throw new JsonReaderException($"The game {game} has no configuration type set.");
            }

            return token.ToObject(type);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IGameConfig).IsAssignableFrom(objectType);
        }

        public override bool CanRead => true;
    }
}