using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using System.ComponentModel;
using System.Windows.Media;

namespace VK_Metro.Views
{
    public partial class Captcha : PhoneApplicationPage, INotifyPropertyChanged
    {
        public bool EnterButtonEnabled { get; private set; }
        public Brush ColorTextEnterButton { get; private set; }
        public ImageSource captchaSource { get; private set; }

        public Captcha()
        {
            InitializeComponent();
            DataContext = this;
            this.setCaptchaImage();
            EnterButtonEnabled = true;
        }

        private void setCaptchaImage()
        {
            var uri = new Uri(App.VK.captchaImageAddress, UriKind.Absolute);
            this.captchaSource = new BitmapImage(uri);
            NotifyPropertyChanged("captchaSource");
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

        private void NumberPhone_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.EnterButtonEnabled = this.TextCaptcha.Text != "";
            NotifyPropertyChanged("EnterButtonEnabled");
        }

        private void EnterButtonClick(object sender, RoutedEventArgs e)
        {
            App.VK.RepeatLastRequestWithCaptcha(this.TextCaptcha.Text, result =>
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
                res => {});
        }

        private void EnterButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (EnterButton.IsEnabled)
                this.ColorTextEnterButton = (App.Current.Resources["PhoneForegroundBrush"] as Brush);
            else
                this.ColorTextEnterButton = (App.Current.Resources["PhoneDisabledBrush"] as Brush);
            NotifyPropertyChanged("ColorTextEnterButton");
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("loaded");
        }

        private void PhoneApplicationPage_LayoutUpdated(object sender, EventArgs e)
        {
            //MessageBox.Show("updated");
        }

        private void PhoneApplicationPage_GotFocus(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("focus");
        }

        private void GoToCaptchaPage()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => {
                this.setCaptchaImage();
                                                                NavigationService.Navigate(new Uri(
                                                                                               "/Views/Captcha.xaml",
                                                                                               UriKind.Relative));
            });
        }

        private void GoToCodePage()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Views/CodeInput.xaml",
                                                                                               UriKind.Relative)));
        }
    }
}