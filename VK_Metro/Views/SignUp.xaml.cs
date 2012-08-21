using System;
using System.Collections.Generic;
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
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace VK_Metro.Views
{
    public partial class SignUp : PhoneApplicationPage, INotifyPropertyChanged
    {
        public SignUp()
        {
            InitializeComponent();
            DataContext = this;
            SignUpButtonEnabled = false;
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

        public bool SignUpButtonEnabled { get; private set; }
        public Brush ColorTextEnterButton { get; private set; }

        private void SignUpButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (SignUpButton.IsEnabled)
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
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void NumberPhone_TextChanged(object sender, TextChangedEventArgs e) { this.TextChanged(); }
        private void First_Name_TextChanged(object sender, TextChangedEventArgs e) { this.TextChanged(); }
        private void Last_Name_TextChanged(object sender, TextChangedEventArgs e) { this.TextChanged(); }
        private void TextChanged()
        {
            if (CheckNumberPhone(NumberPhone.Text) && CheckFirstName(First_Name.Text) && CheckLastName(Last_Name.Text))
                this.SignUpButtonEnabled = true;
            else
                this.SignUpButtonEnabled = false;
            NotifyPropertyChanged("SignUpButtonEnabled");
        }

        private bool CheckNumberPhone(string numbler)
        {
            return Regex.IsMatch(numbler, "^((8|\\+7)[\\- ]?)?(\\(?\\d{3}\\)?[\\- ]?)?[\\d\\- ]{7,10}$");
        }
        private bool CheckFirstName(string firstName)
        {
            var b = Regex.IsMatch(firstName, "^[а-я][\\-а-я]*[а-я]$", RegexOptions.IgnoreCase);
            return b;
        }
        private bool CheckLastName(string lastName)
        {
            var b = Regex.IsMatch(lastName, "^[а-я][\\-а-я]*[а-я]$", RegexOptions.IgnoreCase);
            return b;
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            App.VK.SignUp(NumberPhone.Text, First_Name.Text, Last_Name.Text,
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
                error =>
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
    }
}
