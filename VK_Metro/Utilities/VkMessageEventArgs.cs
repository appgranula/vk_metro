namespace VK_Metro.Utilities
{
    using System;
    using System.Collections.Generic;

    public class VkMessageEventArgs : EventArgs
    {
        public VkMessageEventArgs(
            int id, 
            MessageStatus flags, 
            int fromId, 
            DateTime ts, 
            string theme, 
            string body, 
            Dictionary<string, string> attaches)
        {
            this.Flags = flags;
            this.Id = id;
            this.FromId = fromId;
            this.Ts = ts;
            this.Theme = theme;
            this.Body = body;
            this.Attaches = attaches;
        }

        public MessageStatus Flags { get; set; }

        public int Id { get; set; }

        public int FromId { get; set; }

        public DateTime Ts { get; set; }

        public string Theme { get; set; }

        public string Body { get; set; }

        public Dictionary<string, string> Attaches { get; set; }
    }
}
