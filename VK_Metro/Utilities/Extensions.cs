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
    }
}
