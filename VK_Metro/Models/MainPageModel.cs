using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Phone.UserData;

namespace VK_Metro.Models
{
    public class MainPageModel : INotifyPropertyChanged
    {
        public MainPageModel()
        {
            this.Init();
        }
        public void Init()
        {
            this.vkFriend = new ObservableCollection<VKFriendModel>();
            this.vkUsers = new ObservableCollection<VKFriendModel>();
            this.phoneContacts = new ObservableCollection<PhoneContactModel>();
            this.vkMessage = new ObservableCollection<VKMessageModel>();
            this.IsDataLoaded = false;
        }

        private ObservableCollection<VKFriendModel> vkFriend;
        private ObservableCollection<PhoneContactModel> phoneContacts;
        private ObservableCollection<VKFriendModel> vkUsers;
        private ObservableCollection<VKMessageModel> vkMessage;

        public bool IsDataLoaded { get; set; }

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

        public IEnumerable PhoneContacts
        {
            get
            {
                return from item in this.phoneContacts
                       group item by item.groupIndex into n
                       orderby n.Key
                       select new GroupFriends<string, PhoneContactModel>(n);
            }
        }



        public IEnumerable VKMessage
        {
            get
            {
                return from item in this.vkMessage
                       group item by item.uid into n
                       orderby n.Key
                       select new GroupDialogs<string, VKMessageModel>(n);
            }
        }


        public string GetPhoto(string uid)
        {

            foreach (var friend in vkFriend)
            {
                if (friend.uid == uid)
                    return friend.photo;
            }
            foreach (var user in vkUsers)
            {
                if (user.uid == uid)
                    return user.photo;
            }
            App.VK.GetUser(uid, result =>
            {
                VKFriendModel user = (VKFriendModel)result;
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (user.photo == "http://vk.com/images/deactivated_c.gif")
                        user.photo = "/VK_Metro;component/Images/deactivated_c.png";
                    this.vkUsers.Add(user);
                    this.NotifyPropertyChanged("VKMessage");
                });
            }, error =>
            {
            });
            return "";
        }
        public string GetName(string uid)
        {

            foreach (var friend in vkFriend)
            {
                if (friend.uid == uid)
                    return friend.name;
            }
            foreach (var user in vkUsers)
            {
                if (user.uid == uid)
                    return user.name;
            }
            App.VK.GetUser(uid, result =>
            {
                VKFriendModel user = (VKFriendModel)result;
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (user.photo == "http://vk.com/images/deactivated_c.gif")
                        user.photo = "/VK_Metro;component/Images/deactivated_c.png";
                    this.vkUsers.Add(user);
                    this.NotifyPropertyChanged("VKMessage");
                });
            }, error =>
            {
            });
            return "";
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
        /// Добавить друзей из vk.com
        /// </summary>
        /// <param name="vkFriends"></param>
        public void AddFriend(VKFriendModel[] vkFriends)
        {
            foreach (var friend in vkFriends)
                this.vkFriend.Add(friend);
            this.NotifyPropertyChanged("VKFriend");
        }
        
        /// <summary>
        /// Добавить контакты из телефона
        /// </summary>
        /// <param name="contacts"></param>

        public void AddContact(Contact[] contacts)
        {
            foreach (var friend in contacts)
            {
                var img = new BitmapImage();
                var pic = friend.GetPicture();
                if (pic != null)
                {
                    img.SetSource(pic);
                }

                var phone = friend.PhoneNumbers.First().PhoneNumber;
                phone = phone.Replace("+", string.Empty);

                this.phoneContacts.Add(new PhoneContactModel
                                           {first_name = friend.DisplayName, photo = img, phone = phone});
            }

            this.NotifyPropertyChanged("PhoneContacts");
        }

        public void AddVkNameToContacts(Newtonsoft.Json.Linq.JArray vkContacts)
        {
            foreach (var contact in vkContacts)
            {
                var vkPhone = contact.SelectToken("phone").ToString();
                foreach (var phoneContact in phoneContacts)
                {
                    if (phoneContact.phone == vkPhone)
                    {
                        phoneContact.vkName = contact.SelectToken("first_name") + " " + contact.SelectToken("last_name");
                    }
                }
            }
            this.NotifyPropertyChanged("PhoneContacts");
        }
        
        public void AddMessage(VKMessageModel[] VKMessage)
        {
            foreach (var message in VKMessage)
                this.vkMessage.Add(message);
            this.NotifyPropertyChanged("VKMessage");
        }
    }
}
