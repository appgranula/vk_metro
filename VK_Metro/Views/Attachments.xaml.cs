using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace VK_Metro.Views
{

    public partial class Attachments : PhoneApplicationPage, INotifyPropertyChanged
    {
        private ObservableCollection<ImageModel> col = new ObservableCollection<ImageModel>();
        public ObservableCollection<ImageModel> Col 
        { 
            get { return col; }
            set { col = value; } 
        }

        public Attachments()
        {
            DataContext = this;
            this.Col = new ObservableCollection<ImageModel>();
            InitializeComponent();
            this.AttachmentsList.ItemsSource = this.Col;
            UpdateLayout();
            this.NotifyPropertyChanged("Col");
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // дух  машины, моё упрямство сильнее твоей тупости
            //IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            //foreach (KeyValuePair<string, string> parameter in parameters)
            //{
            //    var bitmap = new BitmapImage(new Uri(parameter.Value, UriKind.Absolute))
            //    {
            //        CreateOptions = BitmapCreateOptions.None
            //    };

            //    this.Col.Add(new ImageModel() { Address = bitmap });
            //}
            //if (PhoneApplicationService.Current.State.ContainsKey("Attachments"))
            //{
            //    var t = PhoneApplicationService.Current.State["Attachments"] as List<BitmapImage>;
            //    foreach (var image in t)
            //    {
            //        this.Col.Add(new ImageModel() {Address = image});
            //    }
            //}

            this.LoadAttachments();
            this.NotifyPropertyChanged("Col");
            base.OnNavigatedTo(e);
        }

        private void LoadAttachments()
        {
            this.Col.Clear();
            foreach (var attachment in App.Attachments)
            {
                this.Col.Add(new ImageModel(){ Address = attachment});
            }
        }

        private void AdvancedApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            PhotoChooserTask photo = new PhotoChooserTask();
            photo.Completed +=
                (o, photoResult) =>
                {
                    if (photoResult.TaskResult == TaskResult.Cancel)
                    {
                        return;
                    }

                    var bitmap = new BitmapImage(new Uri(photoResult.OriginalFileName, UriKind.Absolute));

                    this.Col.Add(new ImageModel() { Address = bitmap });
                    this.NotifyPropertyChanged("Col");
                    var im = new BitmapImage();
                    im.CreateOptions = BitmapCreateOptions.None;
                    im.SetSource(photoResult.ChosenPhoto);
                    //var wb = new WriteableBitmap(im);
                    //var ms = new MemoryStream();
                    //wb.SaveJpeg(ms, wb.PixelWidth, wb.PixelHeight, 0, 100);
                    //var myBytes = ms.ToArray();
                    App.Attachments.Add(im);
                    this.LoadAttachments();
                };
            photo.ShowCamera = true;
            photo.Show();
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

        private void DeleteAttachButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var itemToDelete = ((sender as Button).DataContext as ImageModel).Address;
            App.Attachments.Remove(itemToDelete);
            this.LoadAttachments();
        }
    }

    public class ImageModel
    {
        public BitmapImage Address { get; set; }
    }
}