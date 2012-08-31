namespace VK_Metro.Utilities
{
    using System;

    public class VkEventArgs : EventArgs
    {
        public VkEventArgs(int id)
        {
            this.ID = id;
        }

        public int ID { get; set; }
    }
}
