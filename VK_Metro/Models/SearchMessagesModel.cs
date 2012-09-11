namespace VK_Metro.Models
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Collections;
    using System.Linq;

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
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                });
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
                VKDialogModel dialog = new VKDialogModel(message);
                this.vkMessage.Add(dialog);
            }
            this.NotifyPropertyChanged("VKMessages");
        }
        public void RefreshChats(VKChatModel[] VKChat) {
            this.vkChat.Clear();
            foreach (var chat in VKChat) 
            {
                this.vkChat.Add(chat);
            }
            this.NotifyPropertyChanged("VKChats");
        }
        public void GetMessagesWithFilter(string query)
        {
            App.VK.GetMessagesWithFilter(query,
                result => Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.RefreshMessages((VKMessageModel[])result);
                }),
                error =>
                { }
            );
        }

        public void GetChatsWithFilter(string query) {
            App.VK.GetChatsWithFilter(query, result => Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.RefreshChats((VKChatModel[])result);
                }),
                error => { }
            );
        }
        
        
    }
}
