
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
    using System.Collections.Generic;

    [JsonObject(MemberSerialization.OptIn)]
    public class VKForwardMessageModel
    {
        [JsonProperty("body")]
        public string body { get; set; }
        [JsonProperty("uid")]
        public string uid { get; set; }
        [JsonProperty("date")]
        public string date { get; set; }
        [JsonProperty("fwd_messages")]
        public List<object> fwd_messages { get; set; }
    }
}
