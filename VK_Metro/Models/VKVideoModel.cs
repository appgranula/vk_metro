using Newtonsoft.Json;

namespace VK_Metro.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VKVideoModel
    {
        [JsonProperty("vid")]
        public string vid { get; set; }

        [JsonProperty("owner_id")]
        public string owner_id { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("duration")]
        public string duration { get; set; }

        [JsonProperty("image")]
        public string image { get; set; }

        [JsonProperty("image_big")]
        public string image_big { get; set; }

        [JsonProperty("image_small")]
        public string image_small { get; set; }

        [JsonProperty("views")]
        public string views { get; set; }

        [JsonProperty("date")]
        public string date { get; set; }
    }
}
