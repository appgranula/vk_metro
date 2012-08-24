using System;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;

namespace VK_Metro.Models
{
    public class GroupFriends<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private IGrouping<TKey, TElement> grouping;
        public GroupFriends(IGrouping<TKey, TElement> unit)
        {
            grouping = unit;
        }
        public TKey Key
        {
            get
            {
                return grouping.Key;
            }
        }
        public IGrouping<TKey, TElement> Grouping
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
            GroupFriends<TKey, TElement> that = obj as GroupFriends<TKey, TElement>;
            return (that != null) && (this.Key.Equals(that.Key));
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public IEnumerator<TElement> GetEnumerator()
        {
            return grouping.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return grouping.GetEnumerator();
        }
        public bool HasItems
        {
            get
            {
                return grouping.Count<TElement>() > 0;
            }
        }
        public Brush GroupBackgroundBrush
        {
            get
            {
                if (HasItems)
                    return (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
                else
                    return (SolidColorBrush)Application.Current.Resources["PhoneChromeBrush"];
            }
        }
        public Visibility Visible
        {
            get
            {
                try
                {
                    var i = Convert.ToInt32(Key);
                    return Visibility.Collapsed;
                }
                catch (FormatException)
                {
                    return Visibility.Visible;
                }
            }
        }
    }
}
