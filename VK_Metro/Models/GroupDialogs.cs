using System.Linq;
using System.Collections.Generic;

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
    }
}
