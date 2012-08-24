using System.Windows;
using Microsoft.Phone.Controls;
using System.Xml.Linq;
using System.Collections.ObjectModel;
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
                                App.VK.GetDialogs(result2 =>
                                {
                                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                                    {
                                        this.dataContext.AddMessage((VKMessageModel[])result2);
                                    });
                                }, error2 => { });
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

    }
}