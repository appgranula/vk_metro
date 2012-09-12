namespace VK_Metro.Views
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.UserData;
    using VK_Metro.Models;
    using VK_Metro.Utilities;

    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            this.dataContext = App.MainPageData;
            DataContext = this.dataContext;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            this.SynchronizeContactsGrid.Visibility=Visibility.Collapsed;
        }

        private MainPageModel dataContext;

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
                                        App.LpListener.NewMessageEvent += this.onMessageReceive;

                                        App.VK.StartCheckiingFriendsRequests(
                                        r =>
                                        {
                                            this.dataContext.AddFriendRequests((List<int>)r);
                                        },
                                        r =>
                                        {
                                        });

                                        this.dataContext.AddDialog((VKMessageModel[])result2);
                                        this.dataContext.AddMessage((VKMessageModel[])result2);
                                    });
                                }, error2 => { });
                            });
                    },
                    error =>
                    {
                    });
                //this.lpListener = new LongPollListener(App.VK);
                //this.lpListener.Start();
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
            if (!e.Results.Any())
            {
                this.ShowError("No contacts found.");
                return;
            }

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

                            this.dataContext.AddVkNameToContacts((List<Dictionary<string, string>>)result);


                        }),
                result =>
                    {
                        var error = (Dictionary<string, string>) result;
                        this.ShowError(error["error_code"].ToString(CultureInfo.InvariantCulture) == "9"
                                           ? "Flood Control Error"
                                           : "Unknown Error");
                    });
        }

        private void PhoneContactsList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedItem = (PhoneContactModel) this.PhoneContactsList.SelectedItem;
            var querry = String.Format(
                "?ContactName={0}&Name={1}&Phone={2}&Uid={3}", 
                HttpUtility.UrlEncode(selectedItem.name),
                HttpUtility.UrlEncode(selectedItem.vkName),
                HttpUtility.UrlEncode(selectedItem.phone),
                HttpUtility.UrlEncode(selectedItem.uid)
                );
            this.GoToContactInfoPage(querry);
        }

        private void GoToContactInfoPage(string querry)
        {
            NavigationService.Navigate(new Uri("/Views/ContactInfo.xaml" + querry, UriKind.Relative));
        }

        private void ShowError(string errorText)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(errorText));
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var item = (VKDialogModel)((Grid)sender).DataContext;
            this.GoToDialogPage(item);
        }

        private void onMessageReceive(object sender, VkMessageEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(
                () =>
                    {
                        App.VK.GetMessage(e.Id.ToString(), result => {
                            App.MainPageData.AddMessage((VKMessageModel[])result);
                            App.MainPageData.AddDialog((VKMessageModel[])result);
                        }, error => { });
                        if (!e.Flags.Outbox)
                        {
                            this.dataContext.UnreadMessages += 1;
                        }
                    });
        }

        private void GoToRequestsPage()
        {
            NavigationService.Navigate(new Uri("/Views/FriendsRequests.xaml", UriKind.Relative));
        }

        private void GoToDialogPage(VKDialogModel item)
        {
            //dataContext.MarkDialogAsRead(item);
            var destination = "/Views/Dialog.xaml";
            destination += string.Format("?UID={0}&Name={1}&mid={2}", item.UID, item.Name, item.Mid);
            NavigationService.Navigate(new Uri(destination, UriKind.Relative));
        }

        private void RequestsStackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.GoToRequestsPage();
        }

        private void FriendList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedItem = (VKFriendModel)this.Contacts.SelectedItem;
            var uid = selectedItem.uid;
            string destination = "/Views/Dialog.xaml";
            destination += String.Format("?UID={0}&Name={1}", uid, selectedItem.name);
            NavigationService.Navigate(new Uri(destination, UriKind.Relative));
        }

        private void ContactsPanel_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //var selectedItem = (VKFriendModel)this.Contacts.SelectedItem;
            
            //var querry = String.Format(
            //    "?ContactName={0}&Name={1}&Phone={2}&Photo={3}&Uid={4}",
            //    HttpUtility.UrlEncode(selectedItem.name),
            //    HttpUtility.UrlEncode(selectedItem.name),
            //    HttpUtility.UrlEncode(string.Empty),
            //    HttpUtility.UrlEncode(selectedItem.photo_big),
            //    HttpUtility.UrlEncode(selectedItem.uid)
            //    );

            //this.GoToContactInfoPage(querry);
        }

        private void StackPanel_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedItem = (sender as StackPanel).DataContext as VKFriendModel;
            var querry = String.Format(
                "?ContactName={0}&Name={1}&Phone={2}&Photo={3}&Uid={4}",
                HttpUtility.UrlEncode(selectedItem.name),
                HttpUtility.UrlEncode(selectedItem.name),
                HttpUtility.UrlEncode(string.Empty),
                HttpUtility.UrlEncode(selectedItem.photo_big),
                HttpUtility.UrlEncode(selectedItem.uid)
            );

            this.GoToContactInfoPage(querry);
        }

        private void searchMessageButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SearchMessage.xaml", UriKind.Relative));
        }
       
        private void NewMessage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.PivotApp.SelectedIndex = 0;
        }

        private void SearchFriendsButton_Click(object sender, EventArgs e)
        {
            if(this.dataContext.IsDataLoaded)
                NavigationService.Navigate(new Uri("/Views/SearchFriends.xaml", UriKind.Relative));
        }

    }
}
