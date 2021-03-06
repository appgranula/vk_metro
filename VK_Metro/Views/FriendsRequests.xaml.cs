﻿namespace VK_Metro.Views
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Net;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using VK_Metro.Models;

    public partial class FriendsRequests : PhoneApplicationPage, INotifyPropertyChanged
    {
        private ObservableCollection<VKFriendModel> requests;
        private ObservableCollection<VKFriendModel> possibleFriends1;

        public FriendsRequests()
        {
            this.InitializeComponent();
            this.DataContext = this;
            App.MainPageData.PropertyChanged += this.MainPageDataOnPropertyChanged;
            this.RequestCountString = Localization.AppResources.FriendsRequests + " (0)";
            this.possibleFriends1 = App.MainPageData.PossibleFriends;
            this.requests = App.MainPageData.VkFriendsRequests;
            if (this.possibleFriends1.Count == 0) this.MakeRequestForPossibleFriends();
            this.UpdateLayout();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<VKFriendModel> Requests
        {
            get { return this.requests; }
        }

        public ObservableCollection<VKFriendModel> PossibleFriends1
        {
            get { return this.possibleFriends1; }
        }

        public string RequestCountString { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            this.UpdateLayout();
            this.RequestCountString = Localization.AppResources.FriendsRequests + " (" + this.Requests.Count.ToString() + ")";
            this.NotifyPropertyChanged("RequestCountString");
            base.OnNavigatedTo(args);
        }
        
        private void MainPageDataOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "VkFriendsRequests")
            {
                this.RequestCountString = Localization.AppResources.FriendsRequests + " (" + this.Requests.Count.ToString() + ")";
                this.NotifyPropertyChanged("RequestCountString");
                this.NotifyPropertyChanged("Requests");
            }

            if (propertyChangedEventArgs.PropertyName == "PossibleFriends")
            {
                this.NotifyPropertyChanged("PossibleFriends1");
            }

            this.UpdateLayout();
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void MakeRequestForPossibleFriends()
        {
            App.VK.CheckForPossibleFriends(
                res =>
                    {
                        //var result = (List<Dictionary<string, string>>)res;
                        var result = (VKFriendModel[])res;
                        App.MainPageData.AddPossibleFriendsRequests(result);
                    },
                res =>
                    {
                    });
        }

        private void GoToContactInfoPage(string querry)
        {
            NavigationService.Navigate(new Uri("/Views/ContactInfo.xaml" + querry, UriKind.Relative));
        }
        
        private void RequestsList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedItem = (VKFriendModel)this.RequestsList.SelectedItem;
            //this.dataContext.CurrentContact = selectedItem;
            var querry = String.Format(
                "?Request={0}&Name={1}&Photo={2}",
                HttpUtility.UrlEncode(selectedItem.uid),
                HttpUtility.UrlEncode(selectedItem.name),
                HttpUtility.UrlEncode(selectedItem.photo_big)
                );
            this.GoToContactInfoPage(querry);
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SearchGlobal.xaml?FRIENDS=true&CONTACTS=true&OTHERS=true", UriKind.Relative));
        }

        private void MaybeFriendsList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedItem = (VKFriendModel)this.MaybeFriendsList.SelectedItem;
            //this.dataContext.CurrentContact = selectedItem;
            var querry = String.Format(
                "?Request={0}&Name={1}&Photo={2}",
                HttpUtility.UrlEncode(selectedItem.uid),
                HttpUtility.UrlEncode(selectedItem.name),
                HttpUtility.UrlEncode(selectedItem.photo_big)
                );
            this.GoToContactInfoPage(querry);
        }
    }
}
