namespace VK_Metro.Utilities
{
    using System;
    using System.Collections.Generic;

    public delegate void VkEventDelegate(int userId);

    public delegate void UserTypingInConvensionDelegate(int userId, int chatId);

    public delegate void EventWithFlagsDelegate(int userId, int chatId);

    public delegate void NewMessageEventDelegate(int id, int toId, DateTime ts, string theme, string body, Dictionary<string, string> attaches);

    public class LongPollListener
    {
        private VK_API vkApi;

        public LongPollListener(VK_API vkApi)
        {
            this.vkApi = vkApi;
            // test
            //this.NewMessageEvent += this.MyFunc;
            //this.UserTypingEvent += this.MyFunc2;
        }

        public event NewMessageEventDelegate NewMessageEvent;

        public event VkEventDelegate MessageDeleted;

        public event VkEventDelegate UserOnlineEvent;

        public event VkEventDelegate UserOfflineEvent;

        public event VkEventDelegate UserTypingEvent;

        public event UserTypingInConvensionDelegate  UserTypingInConvensionEvent;

        public event VkEventDelegate FlagsChangedEvent;
        
        public event VkEventDelegate NewFlagsEvent;
        
        public event VkEventDelegate ResetFlagsEvent;

        public void Start()
        {
            this.vkApi.UpdatesArrived += this.OnUpdatesArrived;
            this.vkApi.ConnectToLongPoll();
        }

        private void OnUpdatesArrived(Update updates)
        {
            foreach (var u in updates.updates)
            {
                this.ParseUpdate(u);
            }
        }

        private void ParseUpdate(object[] update)
        {
            switch (Int16.Parse(update[0].ToString()))
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
                        break;
                    }

                case 2:
                    {
                        // new message flags
                        break;
                    }

                case 3:
                    {
                        // reset message flags
                        break;
                    }

                case 4:
                    {
                        // new message
                        this.NewMessageEvent.Invoke(
                            int.Parse(update[1].ToString()), 
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
                        this.UserOfflineEvent.Invoke(int.Parse(update[1].ToString()));
                        break;
                    }

                case 51:
                    {
                        // convension params changed
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
                        break;
                    }
            }
        }

        //public NewMessageEventDelegate MyFunc = (id, toId, ts, theme, body, attaches) =>
        //                                            {
        //                                                int i = 6;
        //                                            };

        //public VkEventDelegate MyFunc2 = id =>
        //                                     {
        //                                         int i = 7;
        //                                     };

    }
}
