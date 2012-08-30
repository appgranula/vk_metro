using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using Microsoft.Phone.UserData;
using VK_Metro.Models;
using System.Collections;
using System;
using System.Windows.Controls;

namespace VK_Metro.Views
{

    public partial class MainPage : PhoneApplicationPage
    {
        // Конструктор
        public MainPage()
        {
            InitializeComponent();

            this.dataContext = App.MainPageData;
            DataContext = this.dataContext;
            // Задайте для контекста данных элемента управления listbox пример данных
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            this.SynchronizeContactsGrid.Visibility=Visibility.Collapsed;
        }

        private MainPageModel dataContext;

        // Загрузка данных для элементов ViewModel
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.dataContext.IsDataLoaded)
            {
                App.VK.GetUsers(result =>
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                this.dataContext.AddFriend((VKFriendModel[])result);
                                this.dataContext.IsDataLoaded = true;
                                App.VK.GetDialogs(result2 =>
                                {
                                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                                    {
                                        this.dataContext.AddDialog((VKMessageModel[])result2);
                                        this.dataContext.AddMessage((VKMessageModel[])result2);
                                    });
                                }, error2 => { });
                            });
                    },
                    error =>
                    {
                    });
            }
            //this.synchronizeButton_Click(new object(), new RoutedEventArgs());
        }

        private void AdvancedApplicationBarMenuItem_Click(object sender, System.EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Exit
            if (NavigationService.CanGoBack)
                while (NavigationService.RemoveBackEntry() != null)
                    NavigationService.RemoveBackEntry();
        }

        private void synchronizeButton_Click(object sender, RoutedEventArgs e)
        {
            var cons = new Contacts();
            cons.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(Contacts_SearchCompleted);
            cons.SearchAsync(String.Empty, FilterKind.None, "Contacts Test #1");
        }

        private void Contacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            // Geting  contacts from phone
            var c = e.Results.AsEnumerable();
            var contacts = new ObservableCollection<Contact>(c);
            var phones = String.Empty;
            for (var i = 0; i < contacts.Count; i++)
            {
                for (var j = 0; j < contacts[i].PhoneNumbers.Count(); j++)
                {
                    if (contacts[i].PhoneNumbers.ElementAt(j).PhoneNumber.Length >= 10)
                    {
                        phones += contacts[i].PhoneNumbers.ElementAt(j).PhoneNumber + ", ";
                    }
                }
            }
            phones = phones.Remove(phones.Length - 2);

            // Checking for registration in vk.com
            phones = phones.Replace("*", string.Empty);
            phones = phones.Replace("#", string.Empty);

            App.VK.CheckContacts(
                phones, 
                result => Deployment.Current.Dispatcher.BeginInvoke(
                    () =>
                        {
                            // Saving contacts in mainpage_model
                            this.dataContext.AddContact(e.Results.ToArray());
                            this.SynchronizeDialogCanvas.Visibility = Visibility.Collapsed;
                            this.SynchronizeContactsGrid.Visibility = Visibility.Visible;

                            this.dataContext.AddVkNameToContacts((Newtonsoft.Json.Linq.JArray)result);
                        }),
                result =>
                    {
                        var error = (Newtonsoft.Json.Linq.JObject) result;
                        this.ShowError(error["error"]["error_code"].ToString() == "9"
                                           ? "Flood Control Error"
                                           : "Unknown Error");
                    });
        }

        private void PhoneContactsList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedItem = (PhoneContactModel) this.PhoneContactsList.SelectedItem;
            this.dataContext.CurrentContact = selectedItem;
            this.GoToContactInfoPage();
        }

        private void GoToContactInfoPage()
        {
            NavigationService.Navigate(new Uri("/Views/ContactInfo.xaml", UriKind.Relative));
        }

        private void ShowError(string errorText)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show("Flood Control Error"));
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var item = (VKDialogModel)((Grid)sender).DataContext;
            string destination = "/Views/Dialog.xaml";
            destination += String.Format("?UID={0}", item.UID);
            NavigationService.Navigate(new Uri(destination, UriKind.Relative));
        }
    }
}
