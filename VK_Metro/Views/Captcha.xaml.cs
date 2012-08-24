namespace VK_Metro.Views
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Phone.Controls;

    public partial class Captcha : PhoneApplicationPage, INotifyPropertyChanged
    {
        public Captcha()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.SetCaptchaImage();
            this.EnterButtonEnabled = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool EnterButtonEnabled { get; private set; }

        public Brush ColorTextEnterButton { get; private set; }

        public ImageSource CaptchaSource { get; private set; }

        private void SetCaptchaImage()
        {
            var uri = new Uri(App.VK.captchaImageAddress, UriKind.Absolute);
            this.CaptchaSource = new BitmapImage(uri);
            this.NotifyPropertyChanged("CaptchaSource");
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void NumberPhone_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.EnterButtonEnabled = this.TextCaptcha.Text != string.Empty;
            this.NotifyPropertyChanged("EnterButtonEnabled");
        }

        private void EnterButtonClick(object sender, RoutedEventArgs e)
        {
            App.VK.RepeatLastRequestWithCaptcha(
                this.TextCaptcha.Text, 
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
                res =>
                    {
                    });
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

        private void GoToCaptchaPage()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => 
            {
                this.SetCaptchaImage();
                this.TextCaptcha.Text = string.Empty;
                NavigationService.Navigate(new Uri("/Views/Captcha.xaml", UriKind.Relative));
            });
        }

        private void GoToCodePage()
        {
            Deployment.Current.Dispatcher.BeginInvoke(
                () => NavigationService.Navigate(new Uri("/Views/CodeInput.xaml", UriKind.Relative)));
        }
    }
}
