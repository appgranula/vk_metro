using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Shell;

namespace VK_Metro.Views
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Tasks;
    using VK_Metro.Models;
    using WPExtensions;


    public partial class Dialog : PhoneApplicationPage, INotifyPropertyChanged
    {
        private VKMessageModel scrollToMessage;

        private bool online;

        private List<string> attachments = new List<string>();

        private int numberOfAttachments;
        private int NumberOfAttachments
        {
            get { return numberOfAttachments; }
            set
            {
                numberOfAttachments = value;
                (this.bar.ButtonItems.First() as AdvancedApplicationBarIconButton).Visibility = Visibility.Collapsed;
                this.NotifyPropertyChanged("ManageAttachmentsVisibility");
                this.NotifyPropertyChanged("ImageIconVisibility");
                this.NotifyPropertyChanged("ManageAttachmentsIconUri");
                (this.bar.ButtonItems.First() as AdvancedApplicationBarIconButton).Visibility = Visibility.Visible;
            }
        }

        public Dialog()
        {
            InitializeComponent();
            this.DataContext = this;
            App.MainPageData.PropertyChanged += new PropertyChangedEventHandler(MainPageData_PropertyChanged);
            Loaded += new RoutedEventHandler(OnPageLoaded);
        }


        //private List<BitmapImage> attachmentsImage = new List<BitmapImage>();

        public Visibility ImageIconVisibility 
        { 
            get
            {
                //if (this.attachments.Count > 0)
                if (this.numberOfAttachments > 0)
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }

        public Visibility ManageAttachmentsVisibility
        {
            get
            {
                //if (this.attachments.Count > 0)
                if (this.numberOfAttachments > 0)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public string ManageAttachmentsIconUri
        {
            get
            {
                if (this.numberOfAttachments > 0)
                {
                    return "/icons/appbar.attachments-" + this.numberOfAttachments + ".rest.png";
                }
                return "/icons/appbar.attachments-1.rest.png";
            }
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

            this.NumberOfAttachments = App.Attachments.Count;
            //this.NotifyPropertyChanged("ManageAttachmentsVisibility");
            //this.NotifyPropertyChanged("ImageIconVisibility");
            //this.NotifyPropertyChanged("ManageAttachmentsIconUri");
            base.OnNavigatedTo(args);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //if (e.Content is Attachments)
            //{
            //    (e.Content as Attachments).Col.Clear();
            //    foreach (var imModel in this.attachmentsImage)
            //    {
            //        (e.Content as Attachments).Col.Add(new ImageModel() { Address = imModel });
            //    }
            //}
            //App.Attachments.Clear();
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
            string attch = null;
            if (this.numberOfAttachments > 0)
            {
                // make array of byte images
                List<byte[]> byteImages = new List<byte[]>();

                foreach (var attachment in App.Attachments)
                {
                    var wb = new WriteableBitmap(attachment); 
                    var ms = new MemoryStream();
                    wb.SaveJpeg(ms, wb.PixelWidth, wb.PixelHeight, 0, 100);
                    var myBytes = ms.ToArray();
                    byteImages.Add(myBytes);
                }

                App.VK.GetMessagesUploadServer(
                    res =>
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                //var wb = new WriteableBitmap(App.Attachments.First());
                                //var ms = new MemoryStream();
                                //wb.SaveJpeg(ms, wb.PixelWidth,wb.PixelHeight, 0,100);
                                //var myBytes = ms.ToArray();

                                //App.VK.UploadPhotoToServer(
                                //    res.ToString(),
                                //    myBytes,
                                //    uploadPhotoresult =>
                                //        {
                                //            Deployment.Current.Dispatcher.BeginInvoke(
                                //                () =>
                                //                {
                                //                    App.VK.SendMessage(this.UID, textBox.Text,uploadPhotoresult.ToString(),this.ResultOfSend,this.ErrorResultOfSend);
                                //                });
                                //        },
                                //    uploadPhotoresult =>
                                //        {

                                //        });

                                this.UploadAttachmentsAsQuery(
                                    res.ToString(), 
                                    byteImages, 
                                    string.Empty, 
                                    resultOfUploadAsQuery =>
                                    {
                                        Deployment.Current.Dispatcher.BeginInvoke(
                                            () =>
                                            {
                                                App.VK.SendMessage(this.UID, textBox.Text, resultOfUploadAsQuery.ToString(), this.ResultOfSend, this.ErrorResultOfSend);
                                            });
                                    });

                            });
                        },
                    res =>
                        {
                        });
            }
            else
            {
                App.VK.SendMessage(this.UID, textBox.Text, null, this.ResultOfSend, this.ErrorResultOfSend);   
            }
        }

        private void UploadAttachmentsAsQuery(string server, List<byte[]> attachments, string resultIDs, CallBack cb)
        {
            App.VK.UploadPhotoToServer(
                server,
                attachments.First(),
                uploadPhotoresult =>
                {
                    resultIDs += uploadPhotoresult.ToString() + ",";
                    attachments.Remove(attachments.First());
                    if (attachments.Count > 0)
                    {
                        this.UploadAttachmentsAsQuery(server, attachments, resultIDs, cb);
                    }
                    else
                    {
                        resultIDs = resultIDs.Remove(resultIDs.LastIndexOf(","));
                        cb(resultIDs);
                    }
                },
                uploadPhotoresult =>
                {
                });
        }

        private void ResultOfSend(object result)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                this.MessageText.Text = "";
                App.Attachments.Clear();
                this.NumberOfAttachments = 0;
                //this.NotifyPropertyChanged("ImageIconVisibility");
                //this.NotifyPropertyChanged("ManageAttachmentsVisibility");
                //this.NotifyPropertyChanged("ManageAttachmentsIconUri");
                UpdateLayout();
            });
        }

        private void ErrorResultOfSend(object result)
        {
            
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
            App.VK.GetMessagesUploadServer(res =>
            {

                PhotoChooserTask photo = new PhotoChooserTask();
                
                photo.Completed +=
                    (o, photoResult) =>
                    {
                        if (photoResult.TaskResult == TaskResult.Cancel)
                        {
                            return;
                        }
                        
                        if (this.attachments.Count < 10)
                        {
                            this.attachments.Add(photoResult.OriginalFileName);
                            var newBitmapImage = new BitmapImage();
                            newBitmapImage.SetSource(photoResult.ChosenPhoto);
                            App.Attachments.Clear();
                            App.Attachments.Add(newBitmapImage);
                            this.NumberOfAttachments++;
                            //this.NotifyPropertyChanged("ImageIconVisibility");
                            //this.NotifyPropertyChanged("ManageAttachmentsVisibility");
                            //this.NotifyPropertyChanged("ManageAttachmentsIconUri");
                            //this.attachmentsImage.Add(new BitmapImage(new Uri(photoResult.OriginalFileName)));
                            //var newImage =
                                //new BitmapImage(new Uri("/VK_Metro;component/Images/deactivated_c.png", UriKind.Relative));
                            //    new BitmapImage(new Uri(photoResult.OriginalFileName, UriKind.Absolute));
                            //newImage.CreateOptions = BitmapCreateOptions.None;

                            

                            //this.attachmentsImage.Add(newImage);


                            //var alist = new List<BitmapImage>();
                            //alist.Add(new BitmapImage(new Uri(photoResult.OriginalFileName, UriKind.Absolute)));
                            //PhoneApplicationService.Current.State["Attachments"] = alist;
                        }
                        
                        //this.bar.ButtonItems.Remove(this.bar.ButtonItems[1]);
//ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
//b.IsEnabled = false;
                        //var but = (this.bar.ButtonItems[1] as AdvancedApplicationBarIconButton);
                        //var but1 = (this.bar.ButtonItems.Where(
                        //    (item, i) => (item as AdvancedApplicationBarIconButton).Name == "AttachImageBarIcon").First() as AdvancedApplicationBarIconButton);
                        //but1.Visibility = Visibility.Collapsed;
                        //(this.bar.ButtonItems[1] as AdvancedApplicationBarIconButton).Visibility = Visibility.Collapsed;
                        //(this.bar.ButtonItems[2] as AdvancedApplicationBarIconButton).Visibility = Visibility.Collapsed;

                        //this.bar.ReCreateAppBar();
                        //this.NotifyPropertyChanged("ImageIconVisibility");
                        //this.NotifyPropertyChanged("ManageAttachmentsVisibility");
                        //this.NotifyPropertyChanged("ManageAttachmentsIconUri");

                        //var im = new BitmapImage();
                        //im.CreateOptions = BitmapCreateOptions.None;
                        //im.SetSource(photoResult.ChosenPhoto);
                        //var wb = new WriteableBitmap(im);
                        ////var param = Convert.ToBase64String(App.VK.MakeBytesFromImage(new WriteableBitmap(im)));
                        //var ms = new MemoryStream();
                        //wb.SaveJpeg(ms, wb.PixelWidth, wb.PixelHeight, 0, 100);
                        //var myBytes = ms.ToArray();

                        //App.VK.UploadPhotoToServer(
                        //    res.ToString(),
                        //    myBytes,
                        //    uploadPhotoresult =>
                        //    {
                        //        int pzshas = 9;
                        //    },
                        //    uploadPhotoresult =>
                        //    {

                        //    });
                    };
                photo.ShowCamera = true;
                photo.Show();
            },
            res => { });
        }

        private void ChooserTask_Completed(object sender, PhotoResult e)
        {
        }

        private void ManageAttachmentsBarIcon_Click(object sender, EventArgs e)
        {
            var query = string.Empty;
            var i = 0;
            foreach (var attachment in attachments)
            {
                query += "Attach" + ++i + "=" + attachment + '&';
            }

            query = query.Remove(query.LastIndexOf('&'));
            //PhoneApplicationService.Current.State["Text"] = new BitmapImage();
            this.GoToAttachmentsView(query);

        }

        private void GoToAttachmentsView(string query)
        {
            NavigationService.Navigate(new Uri("/Views/Attachments.xaml?" + query, UriKind.Relative));
        }

        private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
        {
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