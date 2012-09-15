namespace VK_Metro.Views
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Phone.Controls;

    public partial class CodeInput : PhoneApplicationPage, INotifyPropertyChanged
    {
        public CodeInput()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool EnterButtonEnabled { get; private set; }

        public Brush ColorTextEnterButton { get; private set; }

        private void TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.TextCheck();
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.TextCheck();
        }

        private void TextCheck()
        {
            // DO_CHECK
            // if (this.email.Text.Length >= 6 && this.pass.Password.Length >= 6)
            if (this.CodeText.Text.Length == 4 
                && this.Password.Password.Length >= 6 
                && this.Password.Password == this.PasswordRepeat.Password)
            {
                this.EnterButtonEnabled = true;
            }
            else
            {
                this.EnterButtonEnabled = false;
            }

            this.NotifyPropertyChanged("EnterButtonEnabled");
        }

        private void EnterButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (EnterButton.IsEnabled)
            {
                this.ColorTextEnterButton = App.Current.Resources["PhoneForegroundBrush"] as Brush;
            }
            else
            {
                this.ColorTextEnterButton = App.Current.Resources["PhoneDisabledBrush"] as Brush;
            }

            this.NotifyPropertyChanged("ColorTextEnterButton");
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            App.VK.ConfirmSignUp(
                this.CodeText.Text, 
                this.Password.Password, 
                result =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(
                        () => MessageBox.Show("Вы зарегистрированы. Авторизуйтесь, используя свой логин и пароль"));
                    this.GoToAuthPage();
                }, 
                result => { });
        }

        private void ResendSmsButton_Click(object sender, RoutedEventArgs e)
        {
            App.VK.IDidntReceiveSMS(
                result =>
                {
                    if ((string)result == "captcha")
                    {
                        this.GoToCaptchaPage();
                    }
                    else
                    {
                        this.GoToCodePage();
                    }
                },
                result =>
                {
                });
        }

        private void GoToCaptchaPage()
        {
            Deployment.Current.Dispatcher.BeginInvoke(
                () => NavigationService.Navigate(new Uri("/Views/Captcha.xaml", UriKind.Relative)));
        }

        private void GoToCodePage()
        {
            Deployment.Current.Dispatcher.BeginInvoke(
                () => NavigationService.Navigate(new Uri("/Views/CodeInput.xaml", UriKind.Relative)));
        }

        private void GoToAuthPage()
        {
            Deployment.Current.Dispatcher.BeginInvoke(
                () => NavigationService.Navigate(new Uri("/Views/Authorization.xaml", UriKind.Relative)));
        }
    }
}
