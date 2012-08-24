using Newtonsoft.Json;

namespace VK_Metro.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VKPhotoModel
    {
        [JsonProperty("pid")]
        public string pid { get; set; }

        [JsonProperty("owner_id")]
        public string owner_id { get; set; }

        [JsonProperty("src")]
        public string src { get; set; }

        [JsonProperty("src_big")]
        public string src_big { get; set; }
    }
}
