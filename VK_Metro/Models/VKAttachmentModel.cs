using Newtonsoft.Json;

namespace VK_Metro.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VKAttachmentModel
    {
        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("photo")]
        public VKPhotoModel photo { get; set; }

        [JsonProperty("video")]
        public VKVideoModel video { get; set; }

        [JsonProperty("audio")]
        public VKAudioModel audio { get; set; }

        [JsonProperty("doc")]
        public VKDocModel doc { get; set; }

        [JsonProperty("wall")]
        public string wall { get; set; }
    }
}
