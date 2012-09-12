namespace VK_Metro.Views
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Net;
    using System.Linq;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using VK_Metro.Models;
    using System.Collections;
    using System.Windows.Controls;
    using System.Windows;
    public partial class SearchFriends : PhoneApplicationPage, INotifyPropertyChanged
    {
        private ObservableCollection<VKFriendModel> vkFriends;
        private string query = "";
        public SearchFriends()
        {
            this.DataContext = this;
            this.vkFriends = App.MainPageData.VKFriends;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable VKFriends 
        {
            get
            {
                if (query.Length == 0) 
                {
                    return null;
                }
                return from item in this.vkFriends
                       where (item.name+item.translitName).ToLower().Contains(query.ToLower())
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

        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void FriendsList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var item = (VKFriendModel)FriendsList.SelectedItem;
            var destination = "/Views/Dialog.xaml";
            destination += string.Format("?UID={0}&Name={1}", item.uid, item.name);
            NavigationService.Navigate(new Uri(destination, UriKind.Relative));
        }
        
        private void SearchFriendBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            this.query = ((TextBox)sender).Text;
            NotifyPropertyChanged("VKFriends");
            NotifyPropertyChanged("EmptyTextVisibility");
        }
    }
}