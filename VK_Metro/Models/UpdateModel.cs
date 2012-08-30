namespace VK_Metro.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Update
    {
        [JsonProperty("ts")]
        public string ts { get; set; }

        [JsonProperty("updates")]
        public List<object[]> updates { get; set; }
    }
}
