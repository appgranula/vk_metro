namespace VK_Metro.Views
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.Phone.Controls;
    using VK_Metro.Models;
    using System.Windows.Threading;

    public partial class SearchMessage : PhoneApplicationPage
    {
        private static SearchMessagesModel searchdata = null;
        private DispatcherTimer dispatcherTimer;
        private string currentSearchText;
        public SearchMessage()
        {
            this.InitializeComponent();
            this.DataContext = SearchData;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dispatcherTimer.Tick += new EventHandler(TimerTick);

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
            currentSearchText = ((TextBox)sender).Text;
            dispatcherTimer.Stop();
            dispatcherTimer.Start();
        }
        private void TimerTick(object sender, EventArgs e) 
        {
            if (PivotApp.SelectedIndex == 0)
            {
                SearchData.GetMessagesWithFilter(currentSearchText);
            }
            else
            {
                SearchData.GetChatsWithFilter(currentSearchText);
            }
            dispatcherTimer.Stop();
        }
    }
}
