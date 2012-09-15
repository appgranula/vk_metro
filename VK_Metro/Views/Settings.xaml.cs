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
            DataContext = this;
        }

        public bool IsSoundEnabled 
        {
            get { return App.MainPageData.IsSoundEnabled; }
        }

        public string SoundToggleText 
        {
            get
            {
                if(IsSoundEnabled)
                    return Localization.AppResources.ON;
                else
                    return Localization.AppResources.OFF;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.VK.Disconnect();
            App.MainPageData.Init();
            NavigationService.Navigate(new Uri("/Views/Authorization.xaml", UriKind.Relative));
        }

        private void SoundToggle_Checked(object sender, RoutedEventArgs e)
        {
            SoundToggle.Content = Localization.AppResources.ON;
            App.MainPageData.EnableSound();
        }

        private void SoundToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            SoundToggle.Content = Localization.AppResources.OFF;
            App.MainPageData.DisableSound();
        }
    }
}