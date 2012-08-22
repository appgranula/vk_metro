using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using Microsoft.Phone.UserData;
using VK_Metro.Models;
using System.Collections;
using System;

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
                            });
                    },
                    error =>
                    {
                    });
            }
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
            Contacts cons = new Contacts();

            //Identify the method that runs after the asynchronous search completes.
            cons.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(Contacts_SearchCompleted);

            //Start the asynchronous search.
            cons.SearchAsync(String.Empty, FilterKind.None, "Contacts Test #1");
        }

        void Contacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            MessageBox.Show(e.Results.Count().ToString());
            var c = e.Results.AsEnumerable();
            //var myObservableCollection = new ObservableCollection<Contact>(c);
            var i = new ObservableCollection<Contact>(c);
        }
    }
}