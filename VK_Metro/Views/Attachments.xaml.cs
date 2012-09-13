using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace VK_Metro.Views
{
    public partial class Attachments : PhoneApplicationPage
    {
        public ObservableCollection<string> Col { get; set; }
        public Attachments()
        {
            this.Col = new ObservableCollection<string>();
            this.Col.Add("1");
            this.Col.Add("2");
            this.Col.Add("3");
            this.Col.Add("1");
            this.Col.Add("2");
            this.Col.Add("3");
            this.Col.Add("1");
            this.Col.Add("2");
            this.Col.Add("3");

            InitializeComponent();
            this.AttachmentsList.ItemsSource = this.Col;
            UpdateLayout();
        }
    }
}