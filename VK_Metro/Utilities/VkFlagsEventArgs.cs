namespace VK_Metro.Utilities
{
    using System;

    public class VkFlagsEventArgs : EventArgs
    {
        public VkFlagsEventArgs(int id, MessageStatus messageStatus)
        {
            this.ID = id;
            this.MessageStatus = messageStatus;
        }

        public int ID { get; set; }

        public MessageStatus MessageStatus { get; set; }
    }
}
