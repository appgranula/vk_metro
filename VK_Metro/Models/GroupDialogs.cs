using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.Phone.Controls;

namespace VK_Metro.Models
{
    public class GroupDialogs<TKey, TElement> : IGrouping<string, VKMessageModel>
    {
        private IGrouping<string, VKMessageModel> grouping;
        public GroupDialogs(IGrouping<string, VKMessageModel> unit)
        {
            grouping = unit;
        }
        public string Key
        {
            get
            {
                return grouping.Key;
            }
        }
        public IGrouping<string, VKMessageModel> Grouping
        {
            get
            {
                return grouping;
            }
            set
            {
                grouping = value;
            }
        }
        public override bool Equals(object obj)
        {
            GroupFriends<string, VKMessageModel> that = obj as GroupFriends<string, VKMessageModel>;
            return (that != null) && (this.Key.Equals(that.Key));
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public IEnumerator<VKMessageModel> GetEnumerator()
        {
            return grouping.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return grouping.GetEnumerator();
        }
        public string body
        {
            get
            {
                return grouping.First().body;
            }
        }
        public string photo
        {
            get
            {
                return App.MainPageData.GetPhoto(grouping.First().uid);
            }
        }
        public string name
        {
            get
            {
                return App.MainPageData.GetName(grouping.First().uid);
            }
        }
        public string date
        {
            get
            {
                var dateStr = grouping.Last().date;
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
        public string unixDate
        {
            get
            {
                return grouping.Last().date;
            }
        }
        private string AddZero(int num)
        {
            if (num >= 0 && num <= 9)
                return "0" + num.ToString();
            return num.ToString();
        }
    }
}
