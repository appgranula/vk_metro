using Newtonsoft.Json;

namespace VK_Metro.Models
{
    using System.Collections.Generic;

    [JsonObject(MemberSerialization.OptIn)]
    public class VKFriendModel
    {
        private static Dictionary<string, string> translit = new Dictionary<string, string>();
        
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

        [JsonProperty("timezone")]
        public string timezone { get; set; }

        [JsonProperty("photo")]
        public string photo { get; set; }

        [JsonProperty("photo_medium")]
        public string photo_medium { get; set; }

        [JsonProperty("photo_big")]
        public string photo_big { get; set; }

        [JsonProperty("online")]
        public string online { get; set; }

        [JsonProperty("online_app")]
        public string online_app { get; set; }

        //public string GroupHeader
        //{
        //    get
        //    {
        //        switch (first_name.ToLower().Substring(0, 1))
        //        {
        //            case "a": return "a";
        //            case "b": return "b";
        //            case "c": return "c";
        //            case "d": return "d";
        //            case "e": return "e";
        //            case "f": return "f";
        //            case "g": return "g";
        //            case "h": return "h";
        //            case "i": return "i";
        //            case "j": return "j";
        //            case "k": return "k";
        //            case "l": return "l";
        //            case "m": return "m";
        //            case "n": return "n";
        //            case "o": return "o";
        //            case "p": return "p";
        //            case "q": return "q";
        //            case "r": return "r";
        //            case "s": return "s";
        //            case "t": return "t";
        //            case "u": return "u";
        //            case "v": return "v";
        //            case "w": return "w";
        //            case "x": return "x";
        //            case "y": return "y";
        //            case "z": return "z";
        //            case "а": return "а";
        //            case "б": return "б";
        //            case "в": return "в";
        //            case "г": return "г";
        //            case "д": return "д";
        //            case "е": return "е";
        //            case "ё": return "ё";
        //            case "ж": return "ж";
        //            case "з": return "з";
        //            case "и": return "и";
        //            case "й": return "й";
        //            case "к": return "к";
        //            case "л": return "л";
        //            case "м": return "м";
        //            case "н": return "н";
        //            case "о": return "о";
        //            case "п": return "п";
        //            case "р": return "р";
        //            case "с": return "с";
        //            case "т": return "т";
        //            case "у": return "у";
        //            case "ф": return "ф";
        //            case "х": return "х";
        //            case "ц": return "ц";
        //            case "ч": return "ч";
        //            case "ш": return "ш";
        //            case "щ": return "щ";
        //            case "ъ": return "ъ";
        //            case "ы": return "ы";
        //            case "ь": return "ь";
        //            case "э": return "э";
        //            case "ю": return "ю";
        //            case "я": return "я";
        //            default: return "#";
        //        }
        //    }
        //}
        
        public bool Online
        {
            get { return this.online != "0"; }
        }

        public string name
        {
            get
            {
                var result = "";
                if (first_name != null)
                    result += first_name;
                if (last_name != null)
                    if(result=="")
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
                if (hint != 0)
                {
                    return hint.ToString();
                }
                else
                {
                    if (name != "")
                        return "" + name.ToLower()[0];
                    else
                        return "#";
                }
            }
        }
        public string translitName
        {
            get{ return Translite(name);}
        }
        public int hint { get; set; }
        public static string Translite(string str)
        {
            FillTranslitDictionary();
            if (str.Length > 0)
            {
                str = str.ToLower();
                if (str[0] >= 'a' && str[0] <= 'z')
                {
                    foreach (KeyValuePair<string, string> pair in translit)
                    {
                        str = str.Replace(pair.Value, pair.Key);
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, string> pair in translit)
                    {
                        str = str.Replace(pair.Key, pair.Value);
                    }
                }
            }
            return str;
        }
        private static void FillTranslitDictionary()
        {
            if (translit.Count == 0)
            {
                translit.Add("ч", "ch");
                translit.Add("ш", "sh");
                translit.Add("щ", "sch");
                translit.Add("ё", "yo");
                translit.Add("ж", "zh");
                translit.Add("ю", "yu");
                translit.Add("я", "ya");
                translit.Add("а", "a");
                translit.Add("б", "b");
                translit.Add("в", "v");
                translit.Add("г", "g");
                translit.Add("д", "d");
                translit.Add("е", "e");
                translit.Add("з", "z");
                translit.Add("и", "i");
                translit.Add("й", "j");
                translit.Add("к", "k");
                translit.Add("л", "l");
                translit.Add("м", "m");
                translit.Add("н", "n");
                translit.Add("о", "o");
                translit.Add("п", "p");
                translit.Add("р", "r");
                translit.Add("с", "s");
                translit.Add("т", "t");
                translit.Add("у", "u");
                translit.Add("ф", "f");
                translit.Add("х", "h");
                translit.Add("ц", "c");
                translit.Add("ъ", "j");
                translit.Add("ы", "i");
                translit.Add("ь", "j");
                translit.Add("э", "e");
            }
        }
    }
}
