namespace VK_Metro.Views
{
    using System.Windows;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Tasks;

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

        private void CallButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var phoneCallTask = new PhoneCallTask
                                    {
                                        PhoneNumber = App.MainPageData.CurrentContact.phone,
                                        DisplayName = App.MainPageData.CurrentContact.vkName
                                    };
            phoneCallTask.Show();
        }
    }
}
