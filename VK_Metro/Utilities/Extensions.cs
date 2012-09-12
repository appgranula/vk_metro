using System;
using System.Security.Cryptography;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace VK_Metro
{
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
