namespace VK_Metro.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Windows;
    using Newtonsoft.Json.Linq;

    public class PostSubmitter
    {

        public string url { get; set; }
        public Dictionary<string, object> parameters { get; set; }
        private string boundary = "----------" + DateTime.Now.Ticks.ToString();
        private HttpWebRequest webRequest;

        public void Submit()
        {
            // Prepare web request...
            webRequest = WebRequest.CreateHttp(url);
            webRequest.Method = "POST";
            webRequest.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            webRequest.BeginGetRequestStream(new AsyncCallback(RequestReady), webRequest);

        }

        public event EventHandler<PostResponseEventArgs> PostResponseEvent; 

        private void RequestReady(IAsyncResult asynchronousResult)
        {
            using (Stream postStream = webRequest.EndGetRequestStream(asynchronousResult))
            {
                writeMultipartObject(postStream, parameters);
            }

            webRequest.BeginGetResponse(new AsyncCallback(ResponseReady), webRequest);

        }

        private void ResponseReady(IAsyncResult asynchronousResult)
        {
            try
            {
                using (var response =
                    (HttpWebResponse) webRequest.EndGetResponse(asynchronousResult))
                using (var streamResponse = response.GetResponseStream())
                using (var streamRead = new StreamReader(streamResponse))
                {
                    var responseString = streamRead.ReadToEnd();
                    var success = response.StatusCode == HttpStatusCode.OK;

                    if (responseString != null)
                    {
                        //JObject comes from Newtonsoft.Json ddl. This is a good one if your working with json
                        JObject jsonResponse = JObject.Parse(responseString);
                        //Do stuff with json.....
                        this.OnReponseArrived(new PostResponseEventArgs(
                            jsonResponse.ToString(), 
                            jsonResponse["server"].ToString(), 
                            jsonResponse["photo"].ToString(), 
                            jsonResponse["hash"].ToString()));
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message == "The remote server returned an error: NotFound.")
                {
                    webRequest.Abort();
                    Deployment.Current.Dispatcher.BeginInvoke(
                        delegate()
                            { MessageBox.Show("Unable to connect to server at this time, please try again later"); });
                }
                else
                    Deployment.Current.Dispatcher.BeginInvoke(
                        delegate() { MessageBox.Show("Unable to upload photo at this time, please try again later"); });
                return;
            }
        }


        public void writeMultipartObject(Stream stream, object data)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                if (data != null)
                {
                    foreach (var entry in data as Dictionary<string, object>)
                    {
                        WriteEntry(writer, entry.Key, entry.Value);
                    }
                }
                writer.Write("--");
                writer.Write(boundary);
                writer.WriteLine("--");
                writer.Flush();
            }
        }

        private void WriteEntry(StreamWriter writer, string key, object value)
        {
            if (value != null)
            {
                writer.Write("--");
                writer.WriteLine(boundary);
                if (value is byte[])
                {
                    byte[] ba = value as byte[];

                    writer.WriteLine(@"Content-Disposition: form-data; name=""{0}""; filename=""{1}""", key,
                                     "sentPhoto.jpg");
                    writer.WriteLine(@"Content-Type: application/octet-stream");
                    writer.WriteLine(@"Content-Type: image / jpeg");
                    writer.WriteLine(@"Content-Length: " + ba.Length);
                    writer.WriteLine();
                    writer.Flush();
                    Stream output = writer.BaseStream;

                    output.Write(ba, 0, ba.Length);
                    output.Flush();
                    writer.WriteLine();
                }
                else
                {
                    writer.WriteLine(@"Content-Disposition: form-data; name=""{0}""", key);
                    writer.WriteLine();
                    writer.WriteLine(value.ToString());
                }
            }
        }

        protected virtual void OnReponseArrived(PostResponseEventArgs e)
        {
            var handler = this.PostResponseEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}