namespace VK_Metro.Views
{
    using System;
    using System.Windows;
    using Microsoft.Phone.Controls;

    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.VK.Disconnect();
            App.MainPageData.Init();
            NavigationService.Navigate(new Uri("/Views/Authorization.xaml", UriKind.Relative));
        }
    }
}