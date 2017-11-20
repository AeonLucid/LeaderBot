using Newtonsoft.Json;

namespace LeaderBot.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object input)
        {
            return JsonConvert.SerializeObject(input);
        }
        
        public static string ToJson(this object input, Formatting formatting)
        {
            return JsonConvert.SerializeObject(input, formatting);
        }
        
        public static string ToJson(this object input, Formatting formatting, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(input, formatting, settings);
        }
    }
}