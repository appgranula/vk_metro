using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using VK_Metro.Models;

namespace VK_Metro.Views
{
    public partial class FriendsRequests : PhoneApplicationPage, INotifyPropertyChanged
    {
        public FriendsRequests()
        {
            InitializeComponent();
            DataContext = App.MainPageData;
            //this.requestsPizda = new ObservableCollection<VKFriendModel>();
            //this.RequestsPizda = App.MainPageData.VkFriendsRequests;
            //this.HuiPizda = App.MainPageData.PhoneContacts;
            this.NotifyPropertyChanged("RequestsPizda");
            this.NotifyPropertyChanged("HuiPizda");
            App.MainPageData.PropertyChanged += MainPageDataOnPropertyChanged;
            UpdateLayout();
        }

        private IEnumerable requestsPizda;

        public IEnumerable RequestsPizda { get { return this.requestsPizda; } set { this.requestsPizda = value; } }

        public IEnumerable HuiPizda { get; set; }

        private void MainPageDataOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "VkFriendsRequests")
            {   
                var model = (MainPageModel) sender;
                //this.RequestsPizda = model.VkFriendsRequests.AsEnumerable();
                this.NotifyPropertyChanged("RequestsPizda");
                UpdateLayout();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            //this.RequestsPizda = App.MainPageData.VkFriendsRequests.AsEnumerable();
            //this.requestsPizda.Add(new VKFriendModel(){uid="qweqwewqe"});
            //this.requestsPizda.Add(new VKFriendModel() { uid = "q12321451" });
            //this.requestsPizda.Add(new VKFriendModel() { uid = "qweqwqetfg57489" });
            //this.HuiPizda = this.requestsPizda.AsEnumerable();
            this.NotifyPropertyChanged("RequestsPizda");
            this.NotifyPropertyChanged("HuiPizda");
            this.NotifyPropertyChanged("VkFriendsRequests");
            UpdateLayout();
            base.OnNavigatedTo(args);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}