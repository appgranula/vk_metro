using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            this.vkDialogs = new ObservableCollection<VKDialogModel>();
            this.IsDataLoaded = false;
        }

        private ObservableCollection<VKFriendModel> vkFriend;
        private ObservableCollection<PhoneContactModel> phoneContacts;
        private ObservableCollection<VKFriendModel> vkUsers;
        private ObservableCollection<VKMessageModel> vkMessage;
        private ObservableCollection<VKDialogModel> vkDialogs;

        public bool IsDataLoaded { get; set; }

        public PhoneContactModel CurrentContact { get; set; }

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

        public IEnumerable VKDialogs
        {
            get
            {
                return from item in this.vkDialogs
                       orderby item.unixDate descending
                       select item;
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
            this.GetUser(uid);
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
            this.GetUser(uid);
            return "";
        }
        public bool GetOnline(string uid)
        {
            foreach (var friend in vkFriend)
            {
                if (friend.uid == uid)
                    return friend.online != "0";
            }
            foreach (var user in vkUsers)
            {
                if (user.uid == uid)
                    return user.online != "0";
            }
            this.GetUser(uid);
            return false;
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

        private void GetUser(string uid)
        {
            App.VK.GetUser(uid, result =>
            {
                VKFriendModel user = (VKFriendModel)result;
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.CheckUser(user);
                    this.vkUsers.Add(user);
                    this.NotifyPropertyChanged("VKMessage");
                    this.NotifyPropertyChanged("VKDialogs");
                });
            }, error =>
            {
            });
        }
        private void CheckUser(VKFriendModel user)
        {
            if (user.photo == "http://vk.com/images/deactivated_c.gif")
                user.photo = "/VK_Metro;component/Images/deactivated_c.png";
            if (user.photo == "http://vk.com/images/camera_c.gif")
                user.photo = "/VK_Metro;component/Images/camera_c.png";
        }

        /// <summary>
        /// Добавить друзей из vk.com
        /// </summary>
        /// <param name="vkFriends"></param>
        public void AddFriend(VKFriendModel[] vkFriends)
        {
            foreach (var friend in vkFriends)
            {
                this.CheckUser(friend);
                this.vkFriend.Add(friend);
            }
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
                var img = new BitmapImage(new Uri("/VK_Metro;component/Images/Photo_Placeholder.png", UriKind.RelativeOrAbsolute));
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
                    else
                    {
                        phoneContact.vkName = String.Empty;
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
        public IEnumerable GetMessage(string uid)
        {
            return from item in this.vkMessage
                   where item.uid == uid
                   orderby item.date descending
                   select item;
        } 
        public void AddDialog(VKMessageModel[] VKMessage)
        {
            foreach (var message in VKMessage)
                this.vkDialogs.Add(new VKDialogModel(message));
            this.NotifyPropertyChanged("VKDialogs");
        }
    }
}
