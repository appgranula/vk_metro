namespace VK_Metro.Utilities
{
    using System;
    using System.Collections.Generic;
    using VK_Metro.Models;

    public class LongPollListener
    {
        private VK_API vkApi;

        public LongPollListener(VK_API vkApi)
        {
            this.vkApi = vkApi;
        }

        public event EventHandler<VkEventArgs> UserTypingEvent; 

        public event EventHandler<VkEventArgs> MessageDeleted; 

        public event EventHandler<VkEventArgs> UserOnlineEvent; 

        public event EventHandler<VkFlagsEventArgs> FlagsChangedEvent;

        public event EventHandler<VkFlagsEventArgs> NewFlagsEvent;

        public event EventHandler<VkFlagsEventArgs> ResetFlagsEvent;

        public event EventHandler<VkChatEventArgs> UserTypingInConvensionEvent;

        public event EventHandler<VkCallEventArgs> UserMakeCallEvent; 

        public event EventHandler<VkUserOfflineEventArgs> UserOfflineEvent; 

        public event EventHandler<VkConvensionParamsChangedEventArgs> ConvensionParamsChangedEvent;

        public event EventHandler<VkMessageEventArgs> NewMessageEvent;

        public void Start()
        {
            this.vkApi.UpdatesArrived += this.OnUpdatesArrived;
            this.vkApi.ConnectToLongPoll();
        }

        protected virtual void OnUserTypingEvent(VkEventArgs e)
        {
            var handler = this.UserTypingEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnMessageDeleted(VkEventArgs e)
        {
            var handler = this.MessageDeleted;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnUserOnlineEvent(VkEventArgs e)
        {
            var handler = this.UserOnlineEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnFlagsChangedEvent(VkFlagsEventArgs e)
        {
            var handler = this.FlagsChangedEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnNewFlagsEvent(VkFlagsEventArgs e)
        {
            var handler = this.NewFlagsEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnResetFlagsEvent(VkFlagsEventArgs e)
        {
            var handler = this.ResetFlagsEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnUserTypingInConvensionEvent(VkChatEventArgs e)
        {
            var handler = this.UserTypingInConvensionEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnUserMakeCallEvent(VkCallEventArgs e)
        {
            var handler = this.UserMakeCallEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnUserOfflineEvent(VkUserOfflineEventArgs e)
        {
            var handler = this.UserOfflineEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnConvensionParamsChangedEvent(VkConvensionParamsChangedEventArgs e)
        {
            var handler = this.ConvensionParamsChangedEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnNewMessageEvent(VkMessageEventArgs e)
        {
            var handler = this.NewMessageEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnUpdatesArrived(UpdateModel updates)
        {
            foreach (var u in updates.updates)
            {
                this.ParseUpdate(u);
            }
        }

        private void ParseUpdate(object[] update)
        {
            switch (short.Parse(update[0].ToString()))
            {
                case 0:
                    {
                        // user delete a message
                        this.OnMessageDeleted(new VkEventArgs(int.Parse(update[1].ToString())));
                        break;
                    }

                case 1:
                    {
                        // message flags changed
                        var messageStatus = new MessageStatus();
                        messageStatus.ParseStatus(int.Parse(update[2].ToString()));
                        this.OnFlagsChangedEvent(new VkFlagsEventArgs(int.Parse(update[1].ToString()), messageStatus));
                        break;
                    }

                case 2:
                    {
                        // set up message flags
                        var messageStatus = new MessageStatus(int.Parse(update[2].ToString()));
                        this.OnNewFlagsEvent(new VkFlagsEventArgs(int.Parse(update[1].ToString()), messageStatus));
                        break;
                    }

                case 3:
                    {
                        // reset message flags
                        var messageStatus = new MessageStatus(int.Parse(update[2].ToString()));
                        this.OnResetFlagsEvent(new VkFlagsEventArgs(int.Parse(update[1].ToString()), messageStatus));
                        break;
                    }

                case 4:
                    {
                        // new message
                        var messageStatus = new MessageStatus(int.Parse(update[2].ToString()));
                        this.OnNewMessageEvent(new VkMessageEventArgs(
                            int.Parse(update[1].ToString()),
                            messageStatus,
                            int.Parse(update[3].ToString()),
                            new DateTime(long.Parse(update[4].ToString())),
                            update[5].ToString(),
                            update[6].ToString(),
                            update[7] as Dictionary<string, string>));
                        break;
                    }

                case 8:
                    {
                        // user online
                        this.OnUserOnlineEvent(new VkEventArgs(int.Parse(update[1].ToString())));
                        break;
                    }

                case 9:
                    {
                        // user offline
                        this.OnUserOfflineEvent(new VkUserOfflineEventArgs(int.Parse(update[1].ToString()), int.Parse(update[2].ToString())));
                        break;
                    }

                case 51:
                    {
                        // convension params changed
                        this.OnConvensionParamsChangedEvent(new VkConvensionParamsChangedEventArgs(int.Parse(update[1].ToString()), update[2].ToString()));
                        break;
                    }

                case 61:
                    {
                        // start typing in dialog
                        this.OnUserTypingEvent(new VkEventArgs(int.Parse(update[1].ToString())));
                        break;
                    }

                case 62:
                    {
                        // start typing in convension
                        this.OnUserTypingInConvensionEvent(new VkChatEventArgs(int.Parse(update[1].ToString()), int.Parse(update[2].ToString())));
                        break;
                    }

                case 70:
                    {
                        // user make call
                        this.OnUserMakeCallEvent(new VkCallEventArgs(int.Parse(update[1].ToString()), int.Parse(update[2].ToString())));
                        break;
                    }
            }
        }
    }
}
