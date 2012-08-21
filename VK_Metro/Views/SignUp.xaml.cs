namespace VK_Metro.Views
{
    using System;
    using System.ComponentModel;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Microsoft.Phone.Controls;

    public partial class SignUp : PhoneApplicationPage, INotifyPropertyChanged
    {
        public SignUp()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.SignUpButtonEnabled = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool SignUpButtonEnabled { get; private set; }

        public Brush ColorTextEnterButton { get; private set; }

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

        private void SignUpButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (SignUpButton.IsEnabled)
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

        private void NumberPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.TextChanged();
        }

        private void First_Name_TextChanged(object sender, TextChangedEventArgs e) 
        { 
            this.TextChanged(); 
        }

        private void Last_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.TextChanged();
        }

        private void TextChanged()
        {
            if (this.CheckNumberPhone(NumberPhone.Text) && this.CheckFirstName(First_Name.Text) && this.CheckLastName(Last_Name.Text))
            {
                this.SignUpButtonEnabled = true;
            }
            else
            {
                this.SignUpButtonEnabled = false;
            }

            this.NotifyPropertyChanged("SignUpButtonEnabled");
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
            App.VK.CheckPhone(
                NumberPhone.Text, 
                result => Deployment.Current.Dispatcher.BeginInvoke(this.ProcessSignUp), 
                result =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(
                        () => MessageBox.Show((string)result));
                    return; 
                });
        }

        private void ProcessSignUp()
        {
            App.VK.SignUp(
                NumberPhone.Text, 
                First_Name.Text, 
                Last_Name.Text, 
                res =>
                {
                    if ((string)res == "captcha")
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
            Deployment.Current.Dispatcher.BeginInvoke(() => NavigationService.Navigate(
                new Uri("/Views/Captcha.xaml", UriKind.Relative)));
        }

        private void GoToCodePage()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => NavigationService.Navigate(
                new Uri("/Views/CodeInput.xaml", UriKind.Relative)));
        }
    }
}
