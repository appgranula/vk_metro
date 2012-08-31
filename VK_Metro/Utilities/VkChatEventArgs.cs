namespace VK_Metro.Utilities
{
    using System;

    public class VkChatEventArgs : EventArgs
    {
        public VkChatEventArgs(int id, int chatId)
        {
            this.ID = id;
            this.ChatID = chatId;
        }

        public int ID { get; set; }

        public int ChatID { get; set; }
    }
}
