using Newtonsoft.Json;

namespace VK_Metro.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VKMessageModel
    {
        [JsonProperty("mid")]
        public string mid { get; set; }

        [JsonProperty("uid")]
        public string uid { get; set; }

        [JsonProperty("date")]
        public string date { get; set; }

        [JsonProperty("read_state")]
        public string read_state { get; set; }

        [JsonProperty("out")]
        public string type { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("body")]
        public string body { get; set; }

        [JsonProperty("attachment")]
        public VKAttachmentModel attachment { get; set; }

        [JsonProperty("attachments")]
        public VKAttachmentModel[] attachments { get; set; }

        [JsonProperty("fwd_messages")]
        public string fwd_messages { get; set; }

        [JsonProperty("chat_id")]
        public string chat_id { get; set; }

        [JsonProperty("chat_active")]
        public string chat_active { get; set; }

        [JsonProperty("users_count")]
        public string users_count { get; set; }

        [JsonProperty("admin_id")]
        public string admin_id { get; set; }

        [JsonProperty("deleted")]
        public string deleted { get; set; }

    }
}
