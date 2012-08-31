namespace VK_Metro.Utilities
{
    using System;

    public class VkUserOfflineEventArgs : EventArgs
    {
        public VkUserOfflineEventArgs(int id, int flags)
        {
            this.ID = id;
            this.Flags = flags;
        }

        public int ID { get; set; }

        public int Flags { get; set; }

    }
}