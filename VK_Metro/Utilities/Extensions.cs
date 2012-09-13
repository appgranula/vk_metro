namespace VK_Metro
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

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
    }
}
