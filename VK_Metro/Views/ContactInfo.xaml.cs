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

namespace VK_Metro.Views
{
    public partial class ContactInfo : PhoneApplicationPage
    {
        public ContactInfo()
        {
            InitializeComponent();
            DataContext = App.MainPageData;
            if (App.MainPageData.CurrentContact.vkName != string.Empty)
            {
                this.RegistredUserInfo.Visibility = Visibility.Visible;
                this.NonRegistredUserInfo.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.RegistredUserInfo.Visibility = Visibility.Collapsed;
                this.NonRegistredUserInfo.Visibility = Visibility.Visible;
            }
        }
    }
}