namespace VK_Metro.Views
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using VK_Metro.Models;

    public partial class Dialog : PhoneApplicationPage, INotifyPropertyChanged
    {
        public Dialog()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string UID { get; private set; }
        public IEnumerable Items { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            if (parameters.ContainsKey("UID"))
            {
                this.UID = parameters["UID"];
                this.NotifyPropertyChanged("UID");
                this.Items = App.MainPageData.GetMessage(this.UID);
                this.NotifyPropertyChanged("Items");
            } 
            base.OnNavigatedTo(args);
        }
    }

    public class MessageContentPresenter : ContentControl
    {
        public DataTemplate MeTemplate { get; set; }
        public DataTemplate YouTemplate { get; set; }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            VKMessageModel message = newContent as VKMessageModel;
            if (message.type == "0")
                ContentTemplate = MeTemplate;
            else
                ContentTemplate = YouTemplate;
        }
    }
}