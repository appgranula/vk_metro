namespace VK_Metro.Utilities
{
    using System;
    using System.Collections.Generic;
    using VK_Metro.Models;

    public delegate void VkEventDelegate(int userId);

    public delegate void VkEventWithFlagsDelegate(int userId, int chatId);

    public delegate void ConvensionParamsChangedEventDelegate(int userId, string self);

    public delegate void NewMessageEventDelegate(int id, int flags, int fromId, DateTime ts, string theme, string body, Dictionary<string, string> attaches);

    public class LongPollListener
    {
        private VK_API vkApi;

        public LongPollListener(VK_API vkApi)
        {
            this.vkApi = vkApi;
        }

        public event VkEventDelegate UserTypingEvent;

        public event VkEventDelegate MessageDeleted;

        public event VkEventDelegate UserOnlineEvent;

        public event VkEventWithFlagsDelegate UserOfflineEvent;

        public event VkEventWithFlagsDelegate UserTypingInConvensionEvent;

        public event VkEventWithFlagsDelegate FlagsChangedEvent;

        public event VkEventWithFlagsDelegate NewFlagsEvent;

        public event VkEventWithFlagsDelegate ResetFlagsEvent;

        public event VkEventWithFlagsDelegate UserMakeCallEvent;

        public event ConvensionParamsChangedEventDelegate ConvensionParamsChangedEvent;

        public event NewMessageEventDelegate NewMessageEvent;

        public void Start()
        {
            this.vkApi.UpdatesArrived += this.OnUpdatesArrived;
            this.vkApi.ConnectToLongPoll();
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
                        this.MessageDeleted.Invoke(int.Parse(update[1].ToString()));
                        break;
                    }

                case 1:
                    {
                        // message flags changed
                        this.FlagsChangedEvent.Invoke(int.Parse(update[1].ToString()), int.Parse(update[2].ToString()));
                        break;
                    }

                case 2:
                    {
                        // new message flags
                        this.NewFlagsEvent.Invoke(int.Parse(update[1].ToString()), int.Parse(update[2].ToString()));
                        break;
                    }

                case 3:
                    {
                        // reset message flags
                        this.ResetFlagsEvent.Invoke(int.Parse(update[1].ToString()), int.Parse(update[2].ToString()));
                        break;
                    }

                case 4:
                    {
                        // new message
                        this.NewMessageEvent.Invoke(
                            int.Parse(update[1].ToString()),
                            int.Parse(update[2].ToString()),
                            int.Parse(update[3].ToString()),
                            new DateTime(long.Parse(update[4].ToString())),
                            update[5].ToString(),
                            update[6].ToString(),
                            update[7] as Dictionary<string, string>);
                        break;
                    }

                case 8:
                    {
                        // user online
                        this.UserOnlineEvent.Invoke(int.Parse(update[1].ToString()));
                        break;
                    }

                case 9:
                    {
                        // user offline
                        this.UserOfflineEvent.Invoke(int.Parse(update[1].ToString()), int.Parse(update[2].ToString()));
                        break;
                    }

                case 51:
                    {
                        // convension params changed
                        this.ConvensionParamsChangedEvent.Invoke(int.Parse(update[1].ToString()), update[2].ToString());
                        break;
                    }

                case 61:
                    {
                        // start typing in dialog
                        this.UserTypingEvent.Invoke(int.Parse(update[1].ToString()));
                        break;
                    }

                case 62:
                    {
                        // start typing in convension
                        this.UserTypingInConvensionEvent.Invoke(int.Parse(update[1].ToString()), int.Parse(update[2].ToString()));
                        break;
                    }

                case 70:
                    {
                        // user make call
                        this.UserMakeCallEvent.Invoke(int.Parse(update[1].ToString()), int.Parse(update[2].ToString()));
                        break;
                    }
            }
        }
    }
}
