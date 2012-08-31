namespace VK_Metro.Utilities
{
    using System;

    public class VkConvensionParamsChangedEventArgs : EventArgs
    {
        public VkConvensionParamsChangedEventArgs(int id, string self)
        {
            this.ID = id;
            this.Self = self;
        }

        public int ID { get; set; }

        public string Self { get; set; }
    }
}
