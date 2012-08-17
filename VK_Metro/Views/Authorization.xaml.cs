using System;
using System.Windows;
using Microsoft.Phone.Controls;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Controls;

namespace VK_Metro.Views
{
    public partial class Authorization : PhoneApplicationPage, INotifyPropertyChanged
    {
        public Authorization()
        {
            IsEnabled = false;
            InitializeComponent();
            DataContext = this;
            EnterButtonEnabled = false;
            this.Loaded += new RoutedEventHandler(Authorization_Loaded);
        }

        void Authorization_Loaded(object sender, RoutedEventArgs e)
        {
            App.VK.CheckOldSession(
                result =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
                        IsEnabled = true;
                    });
                },
                error =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        IsEnabled = true;
                    });
                });
        }

        public string TitleImageUri
        {
            get
            {
                var darkVisibility = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];
                if (darkVisibility == Visibility.Visible)
                {
                    return "/VK_Metro;component/Images/dark/VK_logotype.png";
                }
                else
                {
                    return "/VK_Metro;component/Images/light/VK_logotype_Light.png";
                }
            }
        }
        public string EMail { get { return "ravikwow@rambler.ru"; } }
        public string Pass { get { return "G4zOVBnlzU"; } }
        public bool EnterButtonEnabled { get; private set; }
        public Brush ColorTextEnterButton { get; private set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.email.Text != "" && this.pass.Password != "")//если поля не пустые
            {
                App.VK.Connect(this.email.Text, this.pass.Password,
                    result2 =>
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
                        });
                    },
                    error2 =>
                    {
                    });
            }
        }

        private void email_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { this.TextChanged(); }
        private void pass_PasswordChanged(object sender, RoutedEventArgs e) { this.TextChanged(); }
        private void TextChanged()
        {
            if (email.Text.Length != 0 && pass.Password.Length != 0)
                this.EnterButtonEnabled = true;
            else
                this.EnterButtonEnabled = false;
            NotifyPropertyChanged("EnterButtonEnabled");
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

        private void EnterButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(EnterButton.IsEnabled)
                this.ColorTextEnterButton = (App.Current.Resources["PhoneForegroundBrush"] as Brush);
            else
                this.ColorTextEnterButton = (App.Current.Resources["PhoneDisabledBrush"] as Brush);
            NotifyPropertyChanged("ColorTextEnterButton");
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SignUp.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            App.Quit();
            e.Cancel = true;
        }
    }
}