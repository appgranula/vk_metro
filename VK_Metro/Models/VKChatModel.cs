namespace VK_Metro.Models
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class VKChatModel
    {
            [JsonProperty("type")]
            public string type { get; set; }

            [JsonProperty("chat_id")]
            public string chat_id { get; set; }

            [JsonProperty("uid")]
            public string uid { get; set; }

            [JsonProperty("title")]
            public string title { get; set; }

            [JsonProperty("first_name")]
            public string first_name { get; set; }

            [JsonProperty("last_name")]
            public string last_name { get; set; }

            [JsonProperty("attachment")]
            public VKAttachmentModel attachment { get; set; }

            [JsonProperty("users")]
            public string[] users { get; set; }

            public string Photo 
            { 
                get 
                {
                    if (this.uid != null) 
                    {
                        return App.MainPageData.GetPhoto(uid); 
                    }
                    return App.MainPageData.GetPhoto(users[0]);
                }
            }

            public string Title 
            {
                get 
                {
                    if (this.last_name != null) 
                    {
                        return this.first_name + " " + this.last_name;
                    }

                    return this.title;
                }
            }
    }
}
