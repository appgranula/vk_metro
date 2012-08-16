using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;
using System.Linq;
using System.Windows;

namespace VK_Metro.Models
{
    public class MainPageModel : INotifyPropertyChanged
    {
        public MainPageModel()
        {
            this.vkFriend = new ObservableCollection<VKFriendModel>();
        }
        private ObservableCollection<VKFriendModel> vkFriend;

        public string TitleImageUri
        {
            get
            {
                var darkVisibility = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];
                if (darkVisibility == Visibility.Visible)
                {
                    return "/VK_Metro;component/Images/dark/VK_logotype.png";
                }
                else
                {
                    return "/VK_Metro;component/Images/light/VK_logotype_Light.png";
                }
            }
        }

        public IEnumerable VKFriend
        { 
            get
            {
                return from item in this.vkFriend
                       group item by item.groupIndex into n
                       orderby n.Key
                       select new GroupFriends<string, VKFriendModel>(n);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Добавить
        /// </summary>
        /// <param name="VKFriends"></param>
        public void AddFriend(VKFriendModel[] VKFriends)
        {
            foreach (var friend in VKFriends)
                this.vkFriend.Add(friend);
            this.NotifyPropertyChanged("VKFriend");
        }
    }
}
