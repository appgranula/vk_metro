using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace VK_Metro.Views
{
    public partial class CodeInput : PhoneApplicationPage, INotifyPropertyChanged
    {
        public bool EnterButtonEnabled { get; private set; }

        public Brush ColorTextEnterButton { get; private set; }

        public CodeInput()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.TextCheck();
        }

        private void pass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.TextCheck();
        }

        private void TextCheck()
        {
            // DO_CHECK
            //if (this.email.Text.Length >= 6 && this.pass.Password.Length >= 6)
            if (true)
                this.EnterButtonEnabled = true;
            else
                this.EnterButtonEnabled = false;
            NotifyPropertyChanged("EnterButtonEnabled");
        }

        private void EnterButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (EnterButton.IsEnabled)
                this.ColorTextEnterButton = (App.Current.Resources["PhoneForegroundBrush"] as Brush);
            else
                this.ColorTextEnterButton = (App.Current.Resources["PhoneDisabledBrush"] as Brush);
            NotifyPropertyChanged("ColorTextEnterButton");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            App.VK.ConfirmSignUp(this.CodeText.Text, "galort6713", result =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(
                        () =>
                        MessageBox.Show(
                            "Вы зарегистрированы. Авторизуйтесь, используя свой логин и пароль"));
                    this.GoToAuthPage();
                }, result => { });
        }

        private void ResendSmsButton_Click(object sender, RoutedEventArgs e)
        {
            App.VK.IDidntReceiveSMS(result =>
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
            Deployment.Current.Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Views/Captcha.xaml",
                                                                                               UriKind.Relative)));
        }

        private void GoToCodePage()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Views/CodeInput.xaml",
                                                                                               UriKind.Relative)));
        }

        private void GoToAuthPage()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Views/Authorization.xaml",
                                                                                               UriKind.Relative)));
        }
    }
}
