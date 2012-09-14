namespace VK_Metro
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    public static class Extensions
    {
        private static Dictionary<string, string> translit = new Dictionary<string, string>();
        public static string ToUrlData(this Dictionary<string, string> dict)
        {
            var outData = new StringBuilder();
            foreach (var item in dict)
            {
                outData.AppendFormat("{0}={1}&", item.Key, item.Value);
            }
            if (outData.Length != 0)
                outData.Remove(outData.Length - 1, 1);
            return outData.ToString();
        }

        public static ObservableCollection<T> Remove<T>(
            this ObservableCollection<T> coll, Func<T, bool> condition)
        {
            var itemsToRemove = coll.Where(condition).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                coll.Remove(itemToRemove);
            }

            return coll;
        }

        public static string Translite(this string str)
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

        public static string ParseDate(this string str)
        {
            var dateStr = str;
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

        private static string AddZero(int num)
        {
            if (num >= 0 && num <= 9)
                return "0" + num.ToString();
            return num.ToString();
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
