using System.Windows;
using Microsoft.Phone.Controls;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using VK_Metro.Models;
using System.Collections;

namespace VK_Metro.Views
{

    public partial class MainPage : PhoneApplicationPage
    {
        // Конструктор
        public MainPage()
        {
            InitializeComponent();

            this.dataContext = new VK_Metro.Models.MainPageModel();
            DataContext = this.dataContext;
            // Задайте для контекста данных элемента управления listbox пример данных
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        private MainPageModel dataContext;

        // Загрузка данных для элементов ViewModel
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.VK.GetUsers(result =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            this.dataContext.AddFriend((VKFriendModel[])result);
                        });
                },
                error =>
                {
                });
        }
    }
}