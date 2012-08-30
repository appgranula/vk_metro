namespace VK_Metro.Utilities
{
    public class MessageStatus
    {
        public bool Unread { get; set; }

        public bool Outbox { get; set; }

        public bool Replied { get; set; }

        public bool Important { get; set; }

        public bool Chat { get; set; }

        public bool Friends { get; set; }

        public bool Spam { get; set; }

        public bool Deleted { get; set; }

        public bool Fixed { get; set; }

        public bool Media { get; set; }

        public MessageStatus()
        {
        }

        public MessageStatus(int flags)
        {
            this.ParseStatus(flags);
        }

        public void ParseStatus(int flags)
        {

            //if (flags - 512 >= 0)
            //{
            //    flags -= 512;
            //    this.Media = true;
            //}
            //else
            //{
            //    this.Media = false;
            //}

            //this.Media = (flags - 512 >= 0) ? (flags -= 512) : false;

            //if (this.Media = (flags - 512) >= 0) flags -= 512;

            flags = (this.Media = ((flags - 512) >= 0)) ? flags - 512 : flags;
            
            flags = (this.Fixed = ((flags - 256) >= 0)) ? flags - 256 : flags;
            
            flags = (this.Deleted = ((flags - 128) >= 0)) ? flags - 128 : flags;

            flags = (this.Spam = ((flags - 64) >= 0)) ? flags - 64 : flags;

            flags = (this.Friends = ((flags - 32) >= 0)) ? flags - 32 : flags;

            flags = (this.Chat = ((flags - 16) >= 0)) ? flags - 16 : flags;

            flags = (this.Important = ((flags - 8) >= 0)) ? flags - 8 : flags;

            flags = (this.Replied = ((flags - 4) >= 0)) ? flags - 4 : flags;

            flags = (this.Outbox = ((flags - 2) >= 0)) ? flags - 2 : flags;

            this.Unread = ((flags - 1) >= 0);

            //this.Media =        (flags -= 512) >= 0;
            //this.Fixed =        (flags -= 256) >= 0;
            //this.Deleted =      (flags -= 128) >= 0;
            //this.Spam =         (flags -= 64) >= 0;
            //this.Friends =      (flags -= 32) >= 0;
            //this.Chat =         (flags -= 16) >= 0;
            //this.Important =    (flags -= 8) >= 0;
            //this.Replied =      (flags -= 4) >= 0;
            //this.Outbox =       (flags -= 2) >= 0;
            //this.Unread =       (flags -= 1) >= 0;

            //if (flags - 512 >= 0)
            //{
            //    this.Media = true;
            //}

            //if (flags - 256 >= 0)
            //{
            //    this.Fixed = true;
            //}

            //if (flags - 128 >= 0)
            //{
            //    this.Deleted = true;
            //}

            //if (flags - 64 >= 0)
            //{
            //    this.Spam = true;
            //}

            //if (flags - 32 >= 0)
            //{
            //    this.Friends = true;
            //}

            //if (flags - 16 >= 0)
            //{
            //    this.Chat = true;
            //}

            //if (flags - 8 >= 0)
            //{
            //    this.Important = true;
            //}

            //if (flags - 4 >= 0)
            //{
            //    this.Replied = true;
            //}

            //if (flags - 2 >= 0)
            //{
            //    this.Outbox = true;
            //}

            //if (flags - 1 >= 0)
            //{
            //    this.Unread = true;
            //}
        }
    }
}
