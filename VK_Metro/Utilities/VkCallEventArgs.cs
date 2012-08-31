namespace VK_Metro.Utilities
{
    using System;

    public class VkCallEventArgs : EventArgs
    {
        public VkCallEventArgs(int id, int callId)
        {
            this.ID = id;
            this.CallID = callId;
        }

        public int ID { get; set; }

        public int CallID { get; set; }
    }
}
