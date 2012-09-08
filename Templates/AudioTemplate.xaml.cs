namespace Templates
{
    using System.Windows.Controls;
    using System;
    using System.Windows.Media.Imaging;
    using System.Windows;
    using System.Windows.Threading;
    using System.Windows.Media;
    using System.Windows.Data;

    public partial class AudioTemplate : UserControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source",
            typeof(string),
            typeof(AudioTemplate),
            new PropertyMetadata(null));

        public static readonly DependencyProperty PerformerProperty = DependencyProperty.Register(
            "Performer",
            typeof(string),
            typeof(AudioTemplate),
            new PropertyMetadata(null));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(AudioTemplate),
            new PropertyMetadata(""));

        public string Source
        {
            get { return (string)this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        public string Performer
        {
            get { return (string)this.GetValue(PerformerProperty); }
            set { this.SetValue(PerformerProperty, value); }
        }

        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        public AudioTemplate()
        {
            InitializeComponent();
            this.DataContext = this;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Tick);
            /*mediaElement.DownloadProgressChanged += (s, e) =>
            {
                DownloadProgress.Value = mediaElement.DownloadProgress;
            };*/
        }

        void timer_Tick(object sender, EventArgs e)
        {
            TimeLineSlider.Value = mediaElement.Position.TotalSeconds;
        }

        private bool playing = false;
        private DispatcherTimer timer = new DispatcherTimer();
        private bool firstPressPlay = true;

        private void ButtonPlayPause_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.playing)
                this.mediaElement.Pause();
            else
            {
                if (this.firstPressPlay)
                {
                    mediaElement.SetBinding(MediaElement.SourceProperty, new Binding() { Source = this, Path = new PropertyPath("Source") });
                    //this.mediaElement.Source = new Uri(this.Source);
                    this.firstPressPlay = false;
                }
                else
                    this.mediaElement.Play();
            }
        }

        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            TimeLineSlider.Maximum = ((MediaElement)sender).NaturalDuration.TimeSpan.TotalSeconds;
            //DownloadProgress.Maximum = 1;
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.timer.Stop();
            ((MediaElement)sender).Stop();
        }

        private void TimeLineSlider_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mediaElement.Position = new TimeSpan(0, 0, 0, (int)((Slider)sender).Value);
        }

        private void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            switch (((MediaElement)sender).CurrentState)
            {
                case MediaElementState.Paused:
                    this.playing = false;
                    this.timer.Stop();
                    this.IconButton.ImageSource = new BitmapImage(new Uri(@"appbar.transport.play.rest.png", UriKind.Relative));
                    break;
                case MediaElementState.Playing:
                    this.playing = true;
                    this.timer.Start();
                    this.IconButton.ImageSource = new BitmapImage(new Uri(@"appbar.transport.pause.rest.png", UriKind.Relative));
                    break;
                case MediaElementState.Opening:
                    break;
            }
        }
    }
}