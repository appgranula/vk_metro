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
    using System.Linq;

    public partial class Dialog : PhoneApplicationPage, INotifyPropertyChanged
    {
        public Dialog()
        {
            InitializeComponent();
            this.DataContext = this;
            App.MainPageData.PropertyChanged += new PropertyChangedEventHandler(MainPageData_PropertyChanged);
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

        void MainPageData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VKMessage" && this.UID != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.Items = App.MainPageData.GetMessage(this.UID);
                    this.NotifyPropertyChanged("Items");
                    UpdateLayout();
                    ListMessages.ScrollIntoView(ListMessages.Items.Last());
                });
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var item = (TextBox)sender;
            if (item.Text.Length == 0)
            {
                Watermark.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Watermark.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessageText.Focus();
        }

        private void MessageText_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var textBox = (TextBox)sender;
            if ((e.Key == System.Windows.Input.Key.Enter || e.PlatformKeyCode == 10) && textBox.Text.Length != 0)
            {
                App.VK.SendMessage(this.UID, textBox.Text, result =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        textBox.Text = "";
                    });
                }, error => {
                });
            }
        }

        private void ListMessages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((ListBox)sender).SelectedIndex = -1;
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
                ContentTemplate = YouTemplate;
            else
                ContentTemplate = MeTemplate;
        }
    }
}