using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows.Media.Imaging;

namespace VK_Metro.Views
{
    using System.Windows;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Tasks;
    using Microsoft.Phone.UserData;

    public partial class ContactInfo : PhoneApplicationPage
    {
        public string VkName { get; set; }

        public string ContactName { get; set; }

        public string Phone { get; set; }

        public BitmapImage  Picture { get; set; }

        public ContactInfo()
        {
            DataContext = this;
            InitializeComponent();
            this.Picture = new BitmapImage(new Uri("/VK_Metro;component/Images/Photo_Placeholder.png", UriKind.RelativeOrAbsolute));
            //DataContext = App.MainPageData;
        }

        private void CallButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var phoneCallTask = new PhoneCallTask
                                    {
                                        PhoneNumber = App.MainPageData.CurrentContact.phone,
                                        DisplayName = App.MainPageData.CurrentContact.vkName
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
                //NotifyPropertyChanged("ContactName");
                //NotifyPropertyChanged("Phone");
                //NotifyPropertyChanged("Picture");

                if (this.VkName != string.Empty)
                {
                    this.RegistredUserInfo.Visibility = Visibility.Visible;
                    this.NonRegistredUserInfo.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.RegistredUserInfo.Visibility = Visibility.Collapsed;
                    this.NonRegistredUserInfo.Visibility = Visibility.Visible;
                }

                var cons = new Contacts();
                cons.SearchCompleted += this.Contacts_SearchCompleted;
                cons.SearchAsync(this.Phone, FilterKind.PhoneNumber, "Contacts Test #1");
            }


            base.OnNavigatedTo(e);
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
    }
}
