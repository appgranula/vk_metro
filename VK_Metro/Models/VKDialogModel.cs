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
using System.Text.RegularExpressions;

namespace VK_Metro.Models
{
    public class VKDialogModel
    {
        private VKMessageModel vkMessage;
        public VKDialogModel(VKMessageModel message)
        {
            this.vkMessage = message;
        }
        public string UID { get { return vkMessage.uid; } }
        public string Name { get { return App.MainPageData.GetName(vkMessage.uid); } }
        public string Photo { get { return App.MainPageData.GetPhoto(vkMessage.uid); } }
        public string Message { get { return Regex.Replace(vkMessage.body.Replace("<br>", "\n"), "\\<[^\\>]+\\>", ""); } }
        public VKMessageModel VKMessage { get { return vkMessage; } }
        public string Mid { get { return vkMessage.mid; } }
        public string Date { get {
            var dateStr = vkMessage.date;
            var dateInt = Convert.ToInt64(dateStr);
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var d = origin.AddSeconds(dateInt).ToLocalTime();
            var cur = DateTime.Today.ToLocalTime();
            var razn = d.Date - cur;
            var result = "";
            if (razn.Days == 0)
                result = AddZero(d.Hour) + ":" + AddZero(d.Minute);
            else if (razn.Days == -1)
                result = "вчера";
            else
                result = AddZero(d.Day) + "." + AddZero(d.Month);
            return result;
        } }
        public bool Online { get { return App.MainPageData.GetOnline(vkMessage.uid); } }

        public bool Read
        {
            get
            {
                return this.vkMessage.read_state != "0";
            }
            set 
            {
                this.vkMessage.read_state = !value ? "0" : "1";
            }
        }

        public string unixDate
        {
            get
            {
                return vkMessage.date;
            }
        }

        public SolidColorBrush MessageTextColor
        {
            get
            {
                if (!this.Read)
                {
                    var darkVisibility = (Visibility) Application.Current.Resources["PhoneDarkThemeVisibility"];
                    if (darkVisibility == Visibility.Visible)
                    {
                        return (SolidColorBrush) Application.Current.Resources["PhoneAccentBrush"];
                    }
                    //return new SolidColorBrush(Colors.Transparent);
                    return new SolidColorBrush((Color) Application.Current.Resources["PhoneAccentBrush"]);
                }
                return (SolidColorBrush)Application.Current.Resources["PhoneForegroundBrush"];
            }
        }

        private string AddZero(int num)
        {
            if (num >= 0 && num <= 9)
                return "0" + num.ToString();
            return num.ToString();
        }
    }
}
