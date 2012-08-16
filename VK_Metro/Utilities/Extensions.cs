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
        public static string getSIG(this Dictionary<string, string> dict, string api_key, string api_secret)
        {
            var sortData = dict.OrderBy(pair => pair.Key);
            var outData = new StringBuilder();
            outData.AppendFormat("{0}", api_key);
            foreach (var item in sortData)
            {
                outData.AppendFormat("{0}={1}", item.Key, item.Value);
            }
            outData.AppendFormat("{0}", api_secret);

            byte[] bs = System.Text.Encoding.UTF8.GetBytes(outData.ToString());
            MD5Managed md5 = new MD5Managed();
            byte[] hash = md5.ComputeHash(bs);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2").ToLower());
            }

            return sb.ToString();
        }
    }
}
