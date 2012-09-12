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
    using System.Windows.Threading;
    public partial class SearchContacts : PhoneApplicationPage, INotifyPropertyChanged
    {
        private ObservableCollection<PhoneContactModel> phoneContacts;
        private DispatcherTimer dispatcherTimer;
        private string query = "";
        public SearchContacts()
        {
            this.DataContext = this;
            this.phoneContacts = App.MainPageData.PhoneContactsCollection;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dispatcherTimer.Tick += new EventHandler(TimerTick);
            
            
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void SearchContacts_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            this.query = SearchContactsTextBox.Text;
            this.dispatcherTimer.Stop();
            this.dispatcherTimer.Start();
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

        private void TimerTick(object sender, EventArgs e)
        {
            NotifyPropertyChanged("PhoneContacts");
            NotifyPropertyChanged("EmptyTextVisibility");
            this.dispatcherTimer.Stop();
        }
    }
}