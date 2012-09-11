namespace VK_Metro.Models
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
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

            public string Photo { get {
                if (uid != null) {
                    return App.MainPageData.GetPhoto(uid); 
                }
                return App.MainPageData.GetPhoto(users[0]);
                
            } }
            public string Title {
                get {
                    if (last_name != null) {
                        return first_name + " " + last_name;
                    }
                    return title;
                }
            }

    }
}

