using Newtonsoft.Json;

namespace VK_Metro.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VKAudioModel
    {
        [JsonProperty("aid")]
        public string aid { get; set; }

        [JsonProperty("owner_id")]
        public string owner_id { get; set; }

        [JsonProperty("performer")]
        public string performer { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("duration")]
        public string duration { get; set; }

        [JsonProperty("url")]
        public string url { get; set; }
    }
}
