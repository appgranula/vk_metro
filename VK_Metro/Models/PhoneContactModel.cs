using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace VK_Metro.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PhoneContactModel
    {
        [JsonProperty("uid")]
        public string uid { get; set; }

        [JsonProperty("first_name")]
        public string first_name { get; set; }

        [JsonProperty("last_name")]
        public string last_name { get; set; }

        [JsonProperty("screen_name")]
        public string screen_name { get; set; }

        [JsonProperty("sex")]
        public string sex { get; set; }

        [JsonProperty("bdate")]
        public string bdate { get; set; }

        [JsonProperty("photo")]
        public BitmapImage photo { get; set; }

        [JsonProperty("vkName")]
        public string vkName { get; set; }

        [JsonProperty("phone")]
        public string phone { get; set; }

        public string GroupHeader
        {
            get
            {
                switch (first_name.ToLower().Substring(0, 1))
                {
                    case "a": return "a";
                    case "b": return "b";
                    case "c": return "c";
                    case "d": return "d";
                    case "e": return "e";
                    case "f": return "f";
                    case "g": return "g";
                    case "h": return "h";
                    case "i": return "i";
                    case "j": return "j";
                    case "k": return "k";
                    case "l": return "l";
                    case "m": return "m";
                    case "n": return "n";
                    case "o": return "o";
                    case "p": return "p";
                    case "q": return "q";
                    case "r": return "r";
                    case "s": return "s";
                    case "t": return "t";
                    case "u": return "u";
                    case "v": return "v";
                    case "w": return "w";
                    case "x": return "x";
                    case "y": return "y";
                    case "z": return "z";
                    case "а": return "а";
                    case "б": return "б";
                    case "в": return "в";
                    case "г": return "г";
                    case "д": return "д";
                    case "е": return "е";
                    case "ё": return "ё";
                    case "ж": return "ж";
                    case "з": return "з";
                    case "и": return "и";
                    case "й": return "й";
                    case "к": return "к";
                    case "л": return "л";
                    case "м": return "м";
                    case "н": return "н";
                    case "о": return "о";
                    case "п": return "п";
                    case "р": return "р";
                    case "с": return "с";
                    case "т": return "т";
                    case "у": return "у";
                    case "ф": return "ф";
                    case "х": return "х";
                    case "ц": return "ц";
                    case "ч": return "ч";
                    case "ш": return "ш";
                    case "щ": return "щ";
                    case "ъ": return "ъ";
                    case "ы": return "ы";
                    case "ь": return "ь";
                    case "э": return "э";
                    case "ю": return "ю";
                    case "я": return "я";
                    default: return "#";
                }
            }
        }

        public string name
        {
            get
            {
                var result = "";
                if (first_name != null)
                    result += first_name;
                if (last_name != null)
                    if (result == "")
                        result += last_name;
                    else
                        result += " " + last_name;
                return result;
            }
        }

        public string groupIndex
        {
            get
            {
                if (name != "")
                    return "" + name.ToLower()[0];
                else
                    return "#";
            }
        }

        public string transliteName 
        {
            get 
            {
                return name.Translite();
            }
        }
    }
}
