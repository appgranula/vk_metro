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
    using System.Windows.Markup;

    public partial class Dialog : PhoneApplicationPage, INotifyPropertyChanged
    {
        private VKMessageModel scrollToMessage;
        public Dialog()
        {
            InitializeComponent();
            this.DataContext = this;
            App.MainPageData.PropertyChanged += new PropertyChangedEventHandler(MainPageData_PropertyChanged);
            Loaded += new RoutedEventHandler(OnPageLoaded);
        }

        public string UID { get; private set; }

        public IEnumerable Items { get; private set; }

        public string UserName { get; private set; }

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
                this.UserName = parameters["Name"].ToUpper();
                this.NotifyPropertyChanged("UID");
                this.Items = App.MainPageData.GetMessage(this.UID);
                this.NotifyPropertyChanged("Items");
                this.NotifyPropertyChanged("UserName");
                if (parameters.ContainsKey("mid"))
                {
                    scrollToMessage = App.MainPageData.GetMessageByMid(parameters["mid"]);
                }
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
        void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.ListMessages.ScrollIntoView(scrollToMessage);
                ListBoxItem listBoxItem = (ListBoxItem)this.ListMessages.ItemContainerGenerator.ContainerFromItem(scrollToMessage);
                scrollToMessage = null;
            });
        }

    }

    public class MessageContentPresenter : ContentControl
    {
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            VKMessageModel message = newContent as VKMessageModel;
            string xaml =
                    "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                                  "xmlns:contribControls='clr-namespace:WP7Contrib.View.Controls;assembly=WP7Contrib.View.Controls' " +
                                  "xmlns:src='clr-namespace:Templates;assembly=Templates'>";
            if (message.type == "0")
            {   //YouTemplate
                xaml += "<Grid Margin='5, 0, 115, 10' contribControls:GridUtils.RowDefinitions=',,' Width='335'>" +
                            "<Path Data='m 0,0 l 0,12 l 12,0 l -12,-12' Fill='{StaticResource PhoneAccentBrush}' " +
                                  "Margin='5,0,0,0' HorizontalAlignment='Left' Grid.Row='0'/>" +
                            "<Rectangle Fill='{StaticResource PhoneAccentBrush}' Grid.Row='1' Grid.RowSpan='2'/>" +
                            "<StackPanel Grid.Row='1' Orientation='Vertical'>" +
                                "<TextBlock Text='{Binding Path=Message}' HorizontalAlignment='Left' TextWrapping='Wrap' " +
                                           "Margin='10,5,10,0'/>";
                if (message.attachment != null && message.attachment.type == "photo")
                    xaml +=     "<Image Source='{Binding Path=attachment.photo.src_big}' Margin='10' Stretch='Uniform'/>";
                if (message.attachment != null && message.attachment.type == "audio")
                    foreach (var attachment in message.attachments)
                    {
                        xaml += "<src:AudioTemplate Source='" + attachment.audio.url + "' Performer='" + attachment.audio.performer + "' Title='" + attachment.audio.title + "'/>";
                    }
                xaml +=
                            "</StackPanel>" +
                            "<TextBlock Text='{Binding Path=Date}' HorizontalAlignment='Right' " +
                                       "Margin='10,0,10,5' Grid.Row='2'/>" +
                        "</Grid>";
            }
            else
            {   //MeTemplate
                xaml += "<Grid Margin='115, 10, 5, 0' contribControls:GridUtils.RowDefinitions=',,' Width='335'>" +
                            "<Rectangle Fill='{StaticResource PhoneAccentBrush}' Grid.Row='0' Grid.RowSpan='2'/>" +
                            "<StackPanel Grid.Row='0' Orientation='Vertical'>" +
                                "<TextBlock Text='{Binding Path=Message}' HorizontalAlignment='Left' TextWrapping='Wrap' " +
                                           "Margin='10,5,10,0'/>";
                if (message.attachment != null && message.attachment.type == "photo")
                    xaml +=     "<Image Source='{Binding Path=attachment.photo.src_big}' Margin='10' Stretch='Uniform'/>";
                if (message.attachment != null && message.attachment.type == "audio")
                    xaml +=     "<src:AudioTemplate Source='" + message.attachment.audio.url + "' Performer='" + message.attachment.audio.performer + "' Title='" + message.attachment.audio.title + "'/>";
                xaml += "</StackPanel>" +
                            "<TextBlock Text='{Binding Path=Date}' HorizontalAlignment='Right' " +
                                       "Margin='10,0,10,5' Grid.Row='1'/>" +
                            "<Path Data='m 0,0 l 12,0 l 0,12 l -12,-12' Fill='{StaticResource PhoneAccentBrush}' " +
                                  "Margin='0,0,5,0' HorizontalAlignment='Right' Grid.Row='2'/>" +
                        "</Grid>";
            }
            xaml += "</DataTemplate>";
            ContentTemplate = (DataTemplate)XamlReader.Load(xaml);
        }
    }
}