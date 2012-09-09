namespace VK_Metro.Views
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Tasks;
    using Microsoft.Phone.UserData;

    public partial class ContactInfo : PhoneApplicationPage
    {
        private string uid;

        public ContactInfo()
        {
            this.DataContext = this;
            this.InitializeComponent();
            this.Picture = new BitmapImage(new Uri("/VK_Metro;component/Images/Photo_Placeholder.png", UriKind.RelativeOrAbsolute));
        }

        public string VkName { get; set; }

        public string ContactName { get; set; }

        public string Phone { get; set; }

        public BitmapImage Picture { get; set; }

        public Visibility CallButtonVisibility
        {
            get
            {
                return this.Phone == string.Empty ? Visibility.Collapsed : Visibility.Visible;
            }

            private set { }
        }

        private void CallButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var phoneCallTask = new PhoneCallTask
                                    {
                                        PhoneNumber = this.Phone,
                                        DisplayName = this.VkName
                                    };
            phoneCallTask.Show();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            if (parameters.ContainsKey("Name") & parameters.ContainsKey("Phone") & parameters.ContainsKey("ContactName"))
            {

                this.VkName = HttpUtility.UrlDecode(parameters["Name"]);
                this.Phone = HttpUtility.UrlDecode(parameters["Phone"]);
                this.ContactName = HttpUtility.UrlDecode(parameters["ContactName"]);

                if (this.VkName != string.Empty)
                {
                    this.ShowRegistredUserInfo();
                }
                else
                {
                    this.ShowUnRegistredUserInfo();
                }

                var cons = new Contacts();
                cons.SearchCompleted += this.Contacts_SearchCompleted;
                cons.SearchAsync(this.Phone, FilterKind.PhoneNumber, "Contacts Test #1");
            }

            if (parameters.ContainsKey("Request"))
            {
                this.Phone = string.Empty;
                this.uid = HttpUtility.UrlDecode(parameters["Request"]);
                this.ContactName = HttpUtility.UrlDecode(parameters["Name"]);
                this.Picture = new BitmapImage(new Uri(HttpUtility.UrlDecode(parameters["Photo"])));
                this.ShowRequestInfo();
            }

            base.OnNavigatedTo(e);
        }

        private void ShowRegistredUserInfo()
        {
            this.RequestPageTitle.Visibility = Visibility.Collapsed;
            this.RequestInfo.Visibility = Visibility.Collapsed;
            this.NonRegistredUserInfo.Visibility = Visibility.Collapsed;
            this.RegistredUserInfo.Visibility = Visibility.Visible;
        }

        private void ShowUnRegistredUserInfo()
        {
            this.RequestPageTitle.Visibility = Visibility.Collapsed;
            this.RequestInfo.Visibility = Visibility.Collapsed;
            this.RegistredUserInfo.Visibility = Visibility.Collapsed;
            this.NonRegistredUserInfo.Visibility = Visibility.Visible;
        }

        private void ShowRequestInfo()
        {
            this.RequestPageTitle.Visibility = Visibility.Visible;
            this.RequestInfo.Visibility = Visibility.Visible;
            this.RegistredUserInfo.Visibility = Visibility.Collapsed;
            this.NonRegistredUserInfo.Visibility = Visibility.Collapsed;
        }

        private void Contacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            var contacts = e.Results.ToArray();
            var photo = contacts[0].GetPicture();
            if (photo != null)
            {
                this.Picture.SetSource(photo);
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

        private void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            App.VK.AddVkFriend(
                this.uid, 
                res =>
                    {
                        if ((string)res == "2")
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(
                                () =>
                                    {
                                        App.MainPageData.RefreshFriendsList();
                                        this.ShowRegistredUserInfo();
                                    });
                        }
                    }, 
                res =>
                    {
                    });
        }

        private void DeleteFriendButton_Click(object sender, RoutedEventArgs e)
        {
            App.VK.DeleteVkFriend(
                this.uid,
                res =>  
                {
                    if ((string)res == "2")
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(
                            () =>
                                { 
                                    App.MainPageData.RemoveRequestById(this.uid);
                                    NavigationService.GoBack();
                                });
                    }
                },
                res =>
                {
                });
        }
    }
}
