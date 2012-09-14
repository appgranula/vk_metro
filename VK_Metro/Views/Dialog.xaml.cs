using System;
using System.Windows.Media;
using Microsoft.Phone.Tasks;

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

        private bool online;

        public Dialog()
        {
            InitializeComponent();
            this.DataContext = this;
            
            App.MainPageData.PropertyChanged += new PropertyChangedEventHandler(MainPageData_PropertyChanged);
            Loaded += new RoutedEventHandler(OnPageLoaded);
        }

        public string UID { get; private set; }

        public string Mid { get; set; }

        public IEnumerable Items { get; private set; }

        public string UserName { get; private set; }

        public Visibility OnlineVisibility 
        {
            get { return this.online ? Visibility.Visible : Visibility.Collapsed; }
        }

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
                this.online = App.MainPageData.GetOnline(this.UID);
                this.NotifyPropertyChanged("OnlineVisibility");
                if (parameters.ContainsKey("mid"))
                {
                    scrollToMessage = App.MainPageData.GetMessageByMid(parameters["mid"]);
                    this.Mid = parameters["mid"];
                    this.MarkMessagesAsRead();
                }
                this.MarkMessagesAsRead();

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
                    if (ListMessages.Items.Count != 0)
                    {
                        ListMessages.ScrollIntoView(ListMessages.Items.Last());
                    }
                });
            }
        }

        private void MarkMessagesAsRead()
        {
            if (this.Items == null)
            {
                return;
            }

            var readedMessagesMids = string.Empty;
            var count = 0;
            foreach (var item in this.Items)
            {
                var message = item as VKMessageModel;
                if (message.read_state == "0")
                {
                    message.read_state = "1";
                    readedMessagesMids += message.mid + ",";
                    count++;
                }
            }

            if (count > 0)
            {
                readedMessagesMids = readedMessagesMids.Remove(readedMessagesMids.LastIndexOf(","));
                App.VK.MarkAsRead(
                    readedMessagesMids,
                    res =>
                    {
                        if (res.ToString() == "1")
                        {
                            App.MainPageData.MarkDialogAsReadByUid(this.UID);
                            App.MainPageData.UnreadMessages -= count;
                        }
                    },
                    res =>
                    {
                    });
                //App.MainPageData.MarkDialogAsReadByMid(this.Mid);
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
                this.SendMessage(textBox);
            }
        }

        private void SendMessage(TextBox textBox)
        {
            App.VK.SendMessage(this.UID, textBox.Text, result =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    textBox.Text = "";
                });
            }, error =>
            {
            });
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

        private void SendAppBar_Click(object sender, EventArgs e)
        {
            this.SendMessage(this.MessageText);
        }

        private void AttachPictureBarIconButton_Click(object sender, EventArgs e)
        {
            PhotoChooserTask photo = new PhotoChooserTask();
            photo.Completed += this.ChooserTask_Completed;
            photo.ShowCamera = true;

            photo.Show();
        }

        private void ChooserTask_Completed(object sender, PhotoResult e)
        {
            int i = 8;
        }

        private void ListMessages_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            VKMessageModel message = (sender as MenuItem).DataContext as VKMessageModel;
       
            App.VK.DeleteMessages(
                message.mid,
                res => Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    App.MainPageData.RemoveMessage(message);
                    this.Items = App.MainPageData.GetMessage(this.UID);
                    NotifyPropertyChanged("Items");
                }),
                err => 
                {
                });
        }

        private void ResendItem_Click(object sender, RoutedEventArgs e)
        {
            VKMessageModel message = (sender as MenuItem).DataContext as VKMessageModel;
            var destination = "/Views/FriendsCheck.xaml";
            destination += string.Format("?mids={0}", message.mid);
            NavigationService.Navigate(new Uri(destination, UriKind.Relative));
        }

        private void CopyItem_Click(object sender, RoutedEventArgs e)
        {
            VKMessageModel message = (sender as MenuItem).DataContext as VKMessageModel;
            Clipboard.SetText(message.body);
        }
    }

    public class MessageContentPresenter : ContentControl
    {
        private static Color Darken(Color inColor, double inAmount)
        {
            return Color.FromArgb(
              inColor.A,
              (byte)Math.Max(0, inColor.R - 255 * inAmount),
              (byte)Math.Max(0, inColor.G - 255 * inAmount),
              (byte)Math.Max(0, inColor.B - 255 * inAmount));
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            var phoneThemeBrush = (SolidColorBrush) Resources["PhoneAccentBrush"];
            var darkenPhoneColorBrush = Darken(phoneThemeBrush.Color, 0.2).ToString();

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
                if (message.fwd_messages != null)
                {
                    foreach (Dictionary<string, string> fwdmsg in message.fwd_messages)
                    {
                        xaml += "<TextBlock Style = '{StaticResource PhoneTextSubtleStyle}' Margin='11,0,0,0' Text='пересланное сообщение'/>" +
                                "<Grid> " +
                                    "<Grid.ColumnDefinitions>" +
                                        "<ColumnDefinition Width='Auto'/>" +
                                        "<ColumnDefinition Width='Auto'/>" +
                                        "<ColumnDefinition Width='*'/>" +
                                    "</Grid.ColumnDefinitions>" +
                                    "<Path Grid.Column='0' Stretch='Fill' Stroke='Black' StrokeThickness='3' Opacity='0.6'  Width='22' Height='45' Data='M0,0 L0,1' />" +
                                    "<Image Margin='0,0,10,0' Grid.Column='1' Width='40' Height='40' HorizontalAlignment='Left' Source='" + App.MainPageData.GetPhoto(fwdmsg["uid"]) + "'/>" +
                                    "<Grid Grid.Column='2'>" +
                                        "<Grid.RowDefinitions>" +
                                            "<RowDefinition Height='Auto'/>" +
                                            "<RowDefinition Height='*'/>" +
                                        "</Grid.RowDefinitions>" +
                                        "<StackPanel Grid.Row='0' Orientation='Horizontal'>" +
                                            "<TextBlock Text='" + App.MainPageData.GetName(fwdmsg["uid"]) + "'/>" +
                                            "<TextBlock Style = '{StaticResource PhoneTextSubtleStyle}' Margin='7,0,0,0' Text='" + fwdmsg["date"].ParseDate() + "'/>" +
                                        "</StackPanel>" +
                                        "<TextBlock Grid.Row='1' Text='" + fwdmsg["body"] + "'/>" +
                                    "</Grid>" +
                                "</Grid>";

                    }
                }
                xaml +=
                            "</StackPanel>" +
                            "<TextBlock Style = '{StaticResource PhoneTextSubtleStyle}' Text='{Binding Path=Date}' HorizontalAlignment='Right' " +
                                       "Margin='10,0,10,5' Grid.Row='2'/>" +
                        "</Grid>";
            }
            else
            {   //MeTemplate
                xaml += "<Grid Margin='115, 10, 5, 0' contribControls:GridUtils.RowDefinitions=',,' Width='335'>" +
                            "<Rectangle Fill='" + darkenPhoneColorBrush + "' Grid.Row='0' Grid.RowSpan='2'/>" +
                            "<StackPanel Grid.Row='0' Orientation='Vertical'>" +
                                "<TextBlock Text='{Binding Path=Message}' HorizontalAlignment='Left' TextWrapping='Wrap' " +
                                           "Margin='10,5,10,0'/>";
                if (message.attachment != null && message.attachment.type == "photo")
                    xaml +=     "<Image Source='{Binding Path=attachment.photo.src_big}' Margin='10' Stretch='Uniform'/>";
                if (message.attachment != null && message.attachment.type == "audio")
                    xaml +=     "<src:AudioTemplate Source='" + message.attachment.audio.url + "' Performer='" + message.attachment.audio.performer + "' Title='" + message.attachment.audio.title + "'/>";
                if (message.fwd_messages != null)
                {
                    foreach (Dictionary<string, string> fwdmsg in message.fwd_messages) 
                    {
                        xaml += "<TextBlock Style = '{StaticResource PhoneTextSubtleStyle}' Margin='11,0,0,0' Text='пересланное сообщение'/>" +
                                "<Grid> "+
                                    "<Grid.ColumnDefinitions>" +
                                        "<ColumnDefinition Width='Auto'/>" +
                                        "<ColumnDefinition Width='Auto'/>" +
                                        "<ColumnDefinition Width='*'/>" +
                                    "</Grid.ColumnDefinitions>" +
                                    "<Path Grid.Column='0' Stretch='Fill' Stroke='Black' StrokeThickness='3' Opacity='0.6'  Width='22' Height='45' Data='M0,0 L0,1' />"+
                                    "<Image Margin='0,0,10,0' Grid.Column='1' Width='40' Height='40' HorizontalAlignment='Left' Source='" + App.MainPageData.GetPhoto(fwdmsg["uid"]) + "'/>" +
                                    "<Grid Grid.Column='2'>" +
                                        "<Grid.RowDefinitions>" +
                                            "<RowDefinition Height='Auto'/>" +
                                            "<RowDefinition Height='*'/>" +
                                        "</Grid.RowDefinitions>" +
                                        "<StackPanel Grid.Row='0' Orientation='Horizontal'>"+
                                            "<TextBlock Text='" + App.MainPageData.GetName(fwdmsg["uid"]) + "'/>" +
                                            "<TextBlock Style = '{StaticResource PhoneTextSubtleStyle}' Margin='7,0,0,0' Text='"+fwdmsg["date"].ParseDate()+"'/>"+
                                        "</StackPanel>"+
                                        "<TextBlock Grid.Row='1' Text='" + fwdmsg["body"] + "'/>"+
                                    "</Grid>"+
                                "</Grid>";
                        
                    }
                }
                xaml += "</StackPanel>" +
                            "<TextBlock Style = '{StaticResource PhoneTextSubtleStyle}'  Text='{Binding Path=Date}' HorizontalAlignment='Right' " +
                                       "Margin='10,0,10,5' Grid.Row='1'/>" +
                            "<Path Data='m 0,0 l 12,0 l 0,12 l -12,-12' Fill='" + darkenPhoneColorBrush + "' " +
                                  "Margin='0,0,5,0' HorizontalAlignment='Right' Grid.Row='2'/>" +
                        "</Grid>";
            }
            xaml += "</DataTemplate>";
            ContentTemplate = (DataTemplate)XamlReader.Load(xaml);
        }
    }
}