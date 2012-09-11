namespace VK_Metro.Views
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.Phone.Controls;
    using VK_Metro.Models;

    public partial class SearchMessage : PhoneApplicationPage
    {
        private static SearchMessagesModel searchdata = null;

        public SearchMessage()
        {
            this.InitializeComponent();
            this.DataContext = SearchData;
        }

        public static SearchMessagesModel SearchData
        {
            get
            {
                // Отложить создание модели представления до необходимости
                return searchdata ?? (searchdata = new VK_Metro.Models.SearchMessagesModel());
            }
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var item = (VKDialogModel)((Grid)sender).DataContext;
            var destination = "/Views/Dialog.xaml";
            if (App.MainPageData.GetMessageByMid(item.Mid) == null)
            {
                App.MainPageData.AddMessage(new VKMessageModel[] { item.VKMessage });
            }

            destination += string.Format("?UID={0}&Name={1}&mid={2}", item.UID, item.Name, item.Mid);
            NavigationService.Navigate(new Uri(destination, UriKind.Relative));
        }
        
        private void ChatGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var item = (VKChatModel)((Grid)sender).DataContext;
            var destination = "/Views/Dialog.xaml";
            destination += string.Format("?UID={0}&Name={1}", item.uid, item.Title);
            NavigationService.Navigate(new Uri(destination, UriKind.Relative));
        }
        
        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var searchBox = (TextBox)sender;
            if (PivotApp.SelectedIndex == 0)
            {
                SearchData.GetMessagesWithFilter(searchBox.Text);
            }
            else
            {
                SearchData.GetChatsWithFilter(searchBox.Text);
            }
        }
    }
}
