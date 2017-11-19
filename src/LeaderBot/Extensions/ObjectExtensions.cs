using Newtonsoft.Json;

namespace LeaderBot.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object input, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(input, formatting);
        }
    }
}