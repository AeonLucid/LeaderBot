using System.Collections.Generic;
using LeaderBot.Services;
using Newtonsoft.Json;
using MessageType = LeaderBot.Messages.MessageType;

namespace LeaderBot.Config
{
    internal class ChannelConfig
    {
        /// <summary>
        ///     Assigned by initialization at <see cref="DiscordService.InitializeGuild"/>
        /// </summary>
        [JsonIgnore]
        public ulong ChannelId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("messages")]
        public List<MessageType> Messages { get; set; }
    }
}
