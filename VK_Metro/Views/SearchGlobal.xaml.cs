namespace VK_Metro.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using Microsoft.Phone.Controls;
    using System.Collections.ObjectModel;
    using VK_Metro.Models;
    using System.ComponentModel;
    using System.Collections;
    using System.Windows.Threading;
    using System.Windows.Navigation;

    public partial class SearchGlobal : PhoneApplicationPage, INotifyPropertyChanged
    {
        private ObservableCollection<VKFriendModel> vkFriends;

        private ObservableCollection<VKFriendModel> vkGlobal;

        private ObservableCollection<PhoneContactModel> phoneContacts;

        private string query = "";

        private DispatcherTimer dispatcherTimer;

        private Boolean showFriends = false;

        private Boolean showContacts = false;

        private Boolean showOthers = false;

        public SearchGlobal()
        {
            this.DataContext = this;
            this.vkFriends = App.MainPageData.VKFriends;
            this.phoneContacts = App.MainPageData.PhoneContactsCollection;
            this.vkGlobal = new ObservableCollection<VKFriendModel>();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dispatcherTimer.Tick += new EventHandler(TimerTick);
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable VKGlobal
        {
            get
            {
                return from item in this.vkGlobal
                       select item;
            }
        }

        public IEnumerable VKFriends
        {
            get
            {
                if (query.Length == 0)
                {
                    return null;
                }
                return from item in this.vkFriends
                       where (item.name + item.translitName).ToLower().Contains(query.ToLower())
                       select item;
            }
        }

        public IEnumerable PhoneContacts
        {
            get
            {
                if (query.Length == 0)
                {
                    return null;
                }
                return from item in this.phoneContacts
                       where (item.name + item.transliteName).ToLower().Contains(query.ToLower())
                       select item;
            }
        }

        public Visibility EmptyTextVisibility
        {
            get
            {
                if (query.Length == 0)
                    return Visibility.Visible;
                else return Visibility.Collapsed;
            }
        }

        public Visibility ContactsVisibility
        {
            get
            {
                return GetCollectionVisibility(ContactsList.Items.Count);
            }
        }

        public Visibility OthersVisibility
        {
            get
            {
                return GetCollectionVisibility(vkGlobal.Count);
            }
        }

        public Visibility FriendsVisibility
        {
            get
            {
                return GetCollectionVisibility(FriendsList.Items.Count);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            this.showFriends = parameters.ContainsKey("FRIENDS");
            this.showOthers = parameters.ContainsKey("OTHERS");
            this.showContacts = parameters.ContainsKey("CONTACTS");
            base.OnNavigatedTo(args);
        }

        private void FriendsList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var item = (VKFriendModel)FriendsList.SelectedItem;
            var destination = "/Views/Dialog.xaml";
            destination += string.Format("?UID={0}&Name={1}", item.uid, item.name);
            NavigationService.Navigate(new Uri(destination, UriKind.Relative));
        }

        private void ContactsList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedItem = (PhoneContactModel)this.ContactsList.SelectedItem;
            var querry = String.Format(
                "?ContactName={0}&Name={1}&Phone={2}&Uid={3}",
                HttpUtility.UrlEncode(selectedItem.name),
                HttpUtility.UrlEncode(selectedItem.vkName),
                HttpUtility.UrlEncode(selectedItem.phone),
                HttpUtility.UrlEncode(selectedItem.uid)
                );
            NavigationService.Navigate(new Uri("/Views/ContactInfo.xaml" + querry, UriKind.Relative));
        }

        private void OtherList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedItem = (VKFriendModel)this.OtherList.SelectedItem;
            //this.dataContext.CurrentContact = selectedItem;
            var querry = String.Format(
                "?Uid={0}&Name={1}&Photo={2}&AddFriend={3}",
                HttpUtility.UrlEncode(selectedItem.uid),
                HttpUtility.UrlEncode(selectedItem.name),
                HttpUtility.UrlEncode(selectedItem.photo_big),
                HttpUtility.UrlEncode("true")
                );
            NavigationService.Navigate(new Uri("/Views/ContactInfo.xaml" + querry, UriKind.Relative));
        }

        private void GetGlobalUsers(string query) 
        {
            App.VK.SearchUsers(
                query,
                result => Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.RefreshGlobal((VKFriendModel[])result);
                }),
                error => { });
        }

        private void RefreshGlobal(VKFriendModel[] result) 
        {
            this.vkGlobal.Clear();
            foreach (var user in result)
            {
                this.vkGlobal.Add(user);
            }

            this.NotifyPropertyChanged("VKGlobal");
            this.NotifyPropertyChanged("OthersVisibility");
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (null != handler)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }

        private void SearchGlobalTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            this.query = SearchGlobalTextBox.Text;
            this.dispatcherTimer.Stop();
            this.dispatcherTimer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (showOthers) 
            {
                if (this.query.Length != 0)
                {
                    GetGlobalUsers(this.query);
                }
                else
                {
                    this.vkGlobal.Clear();
                    this.NotifyPropertyChanged("VKGlobal");
                    this.NotifyPropertyChanged("OthersVisibility");
                }
            }

            if (showContacts) 
            {
                NotifyPropertyChanged("PhoneContacts");
                NotifyPropertyChanged("ContactsVisibility");
            }

            if (showFriends) 
            {
                NotifyPropertyChanged("VKFriends");
                NotifyPropertyChanged("FriendsVisibility");
            }

            NotifyPropertyChanged("EmptyTextVisibility");
            this.dispatcherTimer.Stop();
        }

        private Visibility GetCollectionVisibility(int count)
        {
            if (this.query.Length == 0)
                return Visibility.Collapsed;
            if (count == 0)
                return Visibility.Collapsed;
            else return Visibility.Visible;
        }
    }
}