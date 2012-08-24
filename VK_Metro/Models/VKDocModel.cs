using Newtonsoft.Json;

namespace VK_Metro.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VKDocModel
    {
        [JsonProperty("did")]
        public string did { get; set; }

        [JsonProperty("owner_id")]
        public string owner_id { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("size")]
        public string size { get; set; }

        [JsonProperty("ext")]
        public string ext { get; set; }

        [JsonProperty("url")]
        public string url { get; set; }
    }
}
