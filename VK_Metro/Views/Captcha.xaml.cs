using System;
using System.Windows;
using Microsoft.Phone.Controls;
using System.ComponentModel;
using System.Windows.Media;

namespace VK_Metro.Views
{
    public partial class Captcha : PhoneApplicationPage, INotifyPropertyChanged
    {
        public bool EnterButtonEnabled { get; private set; }
        public Brush ColorTextEnterButton { get; private set; }

        public Captcha()
        {
            InitializeComponent();
            DataContext = this;
            EnterButtonEnabled = true;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void EnterButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (EnterButton.IsEnabled)
                this.ColorTextEnterButton = (App.Current.Resources["PhoneForegroundBrush"] as Brush);
            else
                this.ColorTextEnterButton = (App.Current.Resources["PhoneDisabledBrush"] as Brush);
            NotifyPropertyChanged("ColorTextEnterButton");
        }
    }
}