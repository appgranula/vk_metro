namespace VK_Metro.Utilities
{
    using System;

    public class PostResponseEventArgs : EventArgs
    {
        public PostResponseEventArgs(string response, string server, string photo, string hash)
        {
            this.Response = response;
            this.Photo = photo;
            this.Server = server;
            this.Hash = hash;
        }

        public string Response { get; set; }

        public string Server { get; set; }

        public string Photo { get; set; }

        public string Hash { get; set; }
    }
}
