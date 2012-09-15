

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
    using System.Collections;
    using System.ComponentModel;

    public partial class FriendsCheck : PhoneApplicationPage, INotifyPropertyChanged
    {
        private ObservableCollection<VKFriendModel> vkFriends;
        private VKFriendModel checkedFriend;
        private string mids;

        public FriendsCheck()
        {
            this.vkFriends = App.MainPageData.VKFriends;
            this.DataContext = this;
            InitializeComponent();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable VKFriends
        {
            get
            {
                return from item in this.vkFriends
                       select item;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            MessageText.Text = Localization.AppResources.Message;
            if (parameters.ContainsKey("mids"))
            {
                this.mids = parameters["mids"];
            }
            base.OnNavigatedTo(e);
        }

        private void FriendsList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ListBoxItem checkedItem = FriendsList.ItemContainerGenerator.ContainerFromItem((sender as RadioButton).DataContext) as ListBoxItem;
            checkedFriend = checkedItem.DataContext as VKFriendModel;
        }

        private void ConfirmFriendButton_Click(object sender, RoutedEventArgs e)
        {
            if (checkedFriend != null)
            {
                MessageText.Text = Localization.AppResources.SendingMessage;
                App.VK.ReSendMessage(
                    ResendMessageBody.Text,
                    checkedFriend.uid,
                    this.mids,
                    res => Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        var destination = "/Views/Dialog.xaml";
                        destination += string.Format("?UID={0}&Name={1}", checkedFriend.uid, checkedFriend.name);
                        NavigationService.Navigate(new Uri(destination, UriKind.Relative));
                    }),
                    err => Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageText.Text = Localization.AppResources.SendingError;
                    }));
            }
        }
    }
}