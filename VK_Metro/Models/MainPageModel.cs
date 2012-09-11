using System.Collections.Specialized;

namespace VK_Metro.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Microsoft.Phone.UserData;

    public class MainPageModel : INotifyPropertyChanged
    {
        private ObservableCollection<VKFriendModel> vkFriend;
        private ObservableCollection<PhoneContactModel> phoneContacts;
        private ObservableCollection<VKFriendModel> vkUsers;
        private ObservableCollection<VKMessageModel> vkMessage;
        private ObservableCollection<VKDialogModel> vkDialogs;
        private int unreadMessages;
        private ObservableCollection<VKFriendModel> vkRequestsFriends;
        private ObservableCollection<VKFriendModel> possibleFriends;

        public MainPageModel()
        {
            this.Init();
        }

        public void MarkDialogAsRead(VKDialogModel dialog)
        {
            dialog.Read = true;
            this.NotifyPropertyChanged("VKDialogs");
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public string UnreadMessagesImageUri
        {
            get
            {
                var darkVisibility = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];
                if (darkVisibility == Visibility.Visible)
                {
                    return "/VK_Metro;component/Images/dark/UnreadMessages_Icon.png";
                }
                return "/VK_Metro;component/Images/light/UnreadMessages_Icon_Light.png";
            }
        }

        public string RequestsImageUri
        {
            get
            {
                var darkVisibility = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];
                if (darkVisibility == Visibility.Visible)
                {
                    return "/VK_Metro;component/Images/dark/Requests_Icon.png";
                }
                return "/VK_Metro;component/Images/light/Requests_Icon_Light.png";
            }
        }

        public int UnreadMessages 
        { 
            get
            {
                return this.unreadMessages;
            }

            set 
            { 
                this.unreadMessages = value;
                this.NotifyPropertyChanged("UnreadMessages");
                this.NotifyPropertyChanged("MessageCounterVisibility");
            } 
        }

        public int FriendsRequests
        {
            get { return this.vkRequestsFriends.Count; }
            private set {}
            //{
            //    this.friendsRequests = value;
            //    this.NotifyPropertyChanged("FriendsRequests");
            //    this.NotifyPropertyChanged("RequestCountVisibility");
            //}
        }

        public ObservableCollection<VKFriendModel> PossibleFriends
        {
            get { return this.possibleFriends; }
        }

        public ObservableCollection<VKFriendModel> VkFriendsRequests 
        {
            get { return this.vkRequestsFriends; }
        }

        public Visibility MessageCounterVisibility
        {
            get
            {
                return this.unreadMessages > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility RequestCountVisibility
        {
            get
            {
                return this.FriendsRequests > 0 ? Visibility.Visible : Visibility.Collapsed;
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

        public void Init()
        {
            this.vkFriend = new ObservableCollection<VKFriendModel>();
            this.vkUsers = new ObservableCollection<VKFriendModel>();
            this.phoneContacts = new ObservableCollection<PhoneContactModel>();
            this.vkMessage = new ObservableCollection<VKMessageModel>();
            this.vkDialogs = new ObservableCollection<VKDialogModel>();
            this.vkRequestsFriends = new ObservableCollection<VKFriendModel>();
            this.possibleFriends = new ObservableCollection<VKFriendModel>();
            this.IsDataLoaded = false;
            this.FriendsRequests = 0;
            this.UnreadMessages = 0;
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

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }

        private void GetUser(string uid)
        {
            App.VK.GetUser(
                uid, 
                result =>
                {
                    VKFriendModel user = (VKFriendModel)result;
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        this.CheckUser(user);
                        this.vkUsers.Add(user);
                        this.NotifyPropertyChanged("VKMessage");
                        this.NotifyPropertyChanged("VKDialogs");
                    });
                }, 
                error =>
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
            if (this.vkFriend.Any())
            {
                this.vkFriend.Clear();
            }

            foreach (var friend in vkFriends)
            {
                this.CheckUser(friend);
                this.vkFriend.Add(friend);
            }

            this.NotifyPropertyChanged("VKFriend");
        }

        public void RefreshFriendsList()
        {
            if (this.IsDataLoaded)
            {
                App.VK.GetUsers(
                    result => Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        this.AddFriend((VKFriendModel[])result);
                        foreach (var friend in this.vkFriend)
                        {
                            foreach (var vkRequestsFriend in this.vkRequestsFriends)
                            {
                                if (vkRequestsFriend.uid == friend.uid)
                                {
                                    this.vkRequestsFriends.Remove(vkRequestsFriend);
                                    break;
                                }
                            }
                        }

                        this.NotifyPropertyChanged("FriendsRequests");
                        this.NotifyPropertyChanged("RequestCountVisibility");
                        this.NotifyPropertyChanged("VkFriendsRequests");
                    }),
                    error =>
                    {
                    });
            }
        }

        public void RemoveRequestById(string uid)
        {
            this.vkRequestsFriends.Remove(x => x.uid == uid);
            this.NotifyPropertyChanged("FriendsRequests");
            this.NotifyPropertyChanged("RequestCountVisibility");
            this.NotifyPropertyChanged("VkFriendsRequests");
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

                var phone = string.Empty;
                if (friend.PhoneNumbers.Count() != 0)
                {
                    phone = friend.PhoneNumbers.First().PhoneNumber;
                    phone = phone.Replace("+", string.Empty);
                }

                this.phoneContacts.Add(new PhoneContactModel
                                           {first_name = friend.DisplayName, photo = img, phone = phone, vkName = string.Empty});
            }

            this.NotifyPropertyChanged("PhoneContacts");
        }

        /// <summary>
        /// Дополняет список телефонных контактов
        /// </summary>
        /// <param name="vkContacts"></param>
        public void AddVkNameToContacts(List<Dictionary<string, string>> vkContacts)
        {
            foreach (var contact in vkContacts)
            {
                var vkPhone = contact["phone"];
                foreach (var phoneContact in phoneContacts)
                {
                    if (phoneContact.phone == vkPhone)
                    {
                        phoneContact.vkName = contact["first_name"] + " " + contact["last_name"];
                        phoneContact.uid = contact["uid"];
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
                   orderby item.date
                   select item;
        } 

        public void AddDialog(VKMessageModel[] VKMessage)
        {
            foreach (var message in VKMessage)
            {
                /*if (message.uid == "146877606")
                {
                    var t = "";
                }*/
                foreach (var dialog in this.vkDialogs)
                    if (dialog.UID == message.uid)
                    {
                        this.vkDialogs.Remove(dialog);
                        break;
                    }
                this.vkDialogs.Add(new VKDialogModel(message));
            }
            this.NotifyPropertyChanged("VKDialogs");
        }

        public void AddFriendRequests(List<int> idList)
        {
            foreach (var vkFriendsRequest in this.vkRequestsFriends)
            {
                if (idList.Contains(int.Parse(vkFriendsRequest.uid)))
                {
                    idList.Remove(int.Parse(vkFriendsRequest.uid));
                }
            }

            if (idList.Count == 0) return;

            foreach (var i in idList)
            {
                var strId = i.ToString(CultureInfo.InvariantCulture);
                App.VK.GetUser(
                strId, 
                result =>
                {
                    var user = (VKFriendModel)result;
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        this.CheckUser(user);
                        this.vkRequestsFriends.Add(user);
                        this.NotifyPropertyChanged("FriendsRequests");
                        this.NotifyPropertyChanged("RequestCountVisibility");
                        this.NotifyPropertyChanged("VkFriendsRequests");
                    });
                }, 
                error =>
                {
                });
            }
        }

        public void AddPossibleFriendsRequests(VKFriendModel[] requests)
        {
            var ids = requests.Aggregate("", (current, user) => current + user.uid + ',');
                ids = ids.Remove(ids.Length - 1);
                App.VK.GetUser(ids, result =>
                {
                    var resultUsers = (VKFriendModel[])result;
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        resultUsers.ToList().ForEach(
                            model =>
                                {
                                    this.CheckUser(model);
                                    this.possibleFriends.Add(model);
                                });
                        this.NotifyPropertyChanged("PossibleFriends");
                    });
                }, error =>
                {
                });
        }
    }
}
