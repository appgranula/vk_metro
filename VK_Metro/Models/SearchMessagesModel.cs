namespace VK_Metro.Models
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;

    public class SearchMessagesModel : INotifyPropertyChanged
    {
        private ObservableCollection<VKDialogModel> vkMessage;
        private ObservableCollection<VKChatModel> vkChat;

        public SearchMessagesModel()
        {
            this.Init();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable VKChats
        {
            get
            {
                return from item in this.vkChat
                       select item;
            }
        }
        
        public IEnumerable VKMessages
        {
            get
            {
                return from item in this.vkMessage
                       orderby item.unixDate descending
                       select item;
            }
        }

        public void Init()
        {
            this.vkMessage = new ObservableCollection<VKDialogModel>();
            this.vkChat = new ObservableCollection<VKChatModel>();
        }

        public void RefreshMessages(VKMessageModel[] VKMessage)
        {
            this.vkMessage.Clear();
            foreach (var message in VKMessage)
            {
                var dialog = new VKDialogModel(message);
                this.vkMessage.Add(dialog);
            }

            this.NotifyPropertyChanged("VKMessages");
        }

        public void RefreshChats(VKChatModel[] VKChat) 
        {
            this.vkChat.Clear();
            foreach (var chat in VKChat) 
            {
                this.vkChat.Add(chat);
            }

            this.NotifyPropertyChanged("VKChats");
        }

        public void GetMessagesWithFilter(string query)
        {
            App.VK.GetMessagesWithFilter(
                query,
                result => Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.RefreshMessages((VKMessageModel[])result);
                }),
                error => { });
        }

        public void GetChatsWithFilter(string query) 
        {
            App.VK.GetChatsWithFilter(
                query, 
                result => Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.RefreshChats((VKChatModel[])result);
                }),
                error => { });
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (null != handler)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }
    }
}
