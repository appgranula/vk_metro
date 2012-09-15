using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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
        public List<object> fwd_messages { get; set; }

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

        private bool check = false;

        public string Checked
        {
            get
            {
                if (this.check)
                {
                    return "true";
                }
                return "false";
            }
            set
            {
                if (value == "True" || value == "true")
                {
                    this.check = true;
                }
                else
                {
                    this.check = false;
                }
            }
        }

        public string Date
        {
            get
            {
                var dateStr = this.date;
                var dateInt = Convert.ToInt64(dateStr);
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                var d = origin.AddSeconds(dateInt).ToLocalTime();
                var cur = DateTime.Today.ToLocalTime();
                var razn = d.Date - cur;
                var result = "";
                if (razn.Days == 0)
                    result = AddZero(d.Hour) + ":" + AddZero(d.Minute);
                else if (razn.Days == -1)
                    result = "вчера";
                else
                    result = AddZero(d.Day) + "." + AddZero(d.Month);
                return result;
            }
        }
        private string AddZero(int num)
        {
            if (num >= 0 && num <= 9)
                return "0" + num.ToString();
            return num.ToString();
        }
        public string Message
        {
            get
            {
                return Regex.Replace(this.body.Replace("<br>", "\n"), "\\<[^\\>]+\\>", "");
            }
        }
    }
}
