namespace VK_Metro.Views
{
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
    using VK_Metro.Models;
    public partial class SearchMessage : PhoneApplicationPage
    {
        private static VK_Metro.Models.SearchMessagesModel searchdata = null;
        public static VK_Metro.Models.SearchMessagesModel SearchData
        {
            get
            {
                // Отложить создание модели представления до необходимости
                if (searchdata == null)
                    searchdata = new VK_Metro.Models.SearchMessagesModel();

                return searchdata;
            }
        }
        public SearchMessage()
        {
            InitializeComponent();

            this.DataContext = SearchData;
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var item = (VKDialogModel)((Grid)sender).DataContext;
            string destination = "/Views/Dialog.xaml";
            if(App.MainPageData.GetMessageByMid(item.Mid)==null)
                App.MainPageData.AddMessage(new VKMessageModel[]{item.VKMessage});
            destination += String.Format("?UID={0}&Name={1}&mid={2}", item.UID, item.Name, item.Mid);
            NavigationService.Navigate(new Uri(destination, UriKind.Relative));
        }
        
        private void ChatGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var item = (VKChatModel)((Grid)sender).DataContext;
            string destination = "/Views/Dialog.xaml";
            destination += String.Format("?UID={0}&Name={1}", item.uid, item.Title);
            NavigationService.Navigate(new Uri(destination, UriKind.Relative));
        }
        
        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox searchBox = (TextBox)sender;
            if (PivotApp.SelectedIndex == 0)
                SearchData.GetMessagesWithFilter(searchBox.Text);
            else
                SearchData.GetChatsWithFilter(searchBox.Text);
        }
    }
}