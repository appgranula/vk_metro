using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using VK_Metro.Models;

namespace VK_Metro
{
    public delegate void EventHandler(Newtonsoft.Json.Linq.JToken updates);

    public delegate void CallBack(object param);

    public class VK_API
    {
        private string access_token;
        private string user_id;
        private CookieContainer cookie;
        private bool _connect;
        public bool connected
        {
            get
            {
                return _connect;
            }
        }

        public VK_API()
        {
            this.cookie = new CookieContainer();
            this._connect = false;
        }

        public void Connect(string email, string password, CallBack onSuccess, CallBack onError)
        {
            string URL = "https://api.vk.com/oauth/token";
            Dictionary<string, string> sendData = new Dictionary<string, string>();
            sendData.Add("grant_type", "password");
            sendData.Add("client_id", "3070837");
            sendData.Add("client_secret", "VykBAOy47loHDoOq9L54");
            sendData.Add("scope", "notify,friends,photos,audio,video,docs,notes,pages,status,offers,questions,wall,groups,messages,notifications,stats,ads,offline");
            sendData.Add("username", email);
            sendData.Add("password", password);
            this.GetQuery(URL, sendData,
                result =>
                {
                    var responseString = (string)result;
                    Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                    this.access_token = obj["access_token"].ToString();
                    this.user_id = obj["user_id"].ToString();
                    this._connect = true;
                    onSuccess(new object());
                },
                error =>
                {
                    onError(error);
                });
        }

        public void GetUsers(CallBack onSuccess, CallBack onError)
        {
            if (this.connected)
            {
                var access_token = this.access_token;
                string URL = "https://api.vk.com/method/friends.get";
                Dictionary<string, string> sendData = new Dictionary<string, string>();
                sendData.Add("access_token", access_token);
                sendData.Add("fields", "uid,first_name,last_name,nickname,screen_name,sex,bdate,timezone,photo,online");
                sendData.Add("order", "hint");
                this.GetQuery(URL, sendData, result =>
                {
                    var responseString = (string)result;
                    Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                    if (obj["response"] != null)
                    {
                        VKFriendModel[] friends = obj["response"].ToObject<VKFriendModel[]>();
                        for (var i = 0; i < 5; i++)
                        {
                            if (friends[i] != null)
                                friends[i].hint = i + 1;
                        }
                        onSuccess(friends);
                    }
                    else
                    {
                        onError(new ObservableCollection<VKFriendModel>());
                    }
                }, error =>
                {
                    onError(error);
                });
            }
        }

        private void PostQuery(string URL, Dictionary<string, string> postData, CallBack onSuccess, CallBack onError)
        {
            HttpWebRequest request = WebRequest.CreateHttp(new Uri(URL));//создаем запрос
            request.ContentType = "application/x-www-form-urlencoded";
            //request.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/msword, application/vnd.ms-excel, application/vnd.ms-powerpoint, */*";
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; ru; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13 ( .NET CLR 3.5.30729)";
            request.Method = "POST";
            request.Headers["Accept-Charset"] = "windows-1251,utf-8;q=0.7,*;q=0.3";
            request.Headers["Accept-Language"] = "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4";
            request.CookieContainer = this.cookie;
            request.BeginGetRequestStream(new AsyncCallback(asynchronousResultRequest =>
            {
                HttpWebRequest requestStateRequest = (HttpWebRequest)asynchronousResultRequest.AsyncState;
                Stream postStream = requestStateRequest.EndGetRequestStream(asynchronousResultRequest);
                byte[] byteArray = Encoding.UTF8.GetBytes(postData.ToUrlData());
                postStream.Write(byteArray, 0, byteArray.Length);
                postStream.Close();
                requestStateRequest.BeginGetResponse(new AsyncCallback(asynchronousResultResponse =>
                {
                    HttpWebRequest requestStateResponse = (HttpWebRequest)asynchronousResultResponse.AsyncState;
                    try
                    {
                        HttpWebResponse response = (HttpWebResponse)requestStateResponse.EndGetResponse(asynchronousResultResponse);
                        var Location = "";
                        if (response.Headers["Location"] != null)
                        {
                            Location = response.Headers["Location"];
                        }
                        Stream streamResponse = response.GetResponseStream();
                        StreamReader streamRead = new StreamReader(streamResponse);
                        string responseString = streamRead.ReadToEnd();
                        streamResponse.Close();
                        streamRead.Close();
                        response.Close();
                        onSuccess(responseString);
                    }
                    catch (WebException e)
                    {
                        onError(e);
                    }
                }), requestStateRequest);
            }), request);
        }

        private void GetQuery(string URL, Dictionary<string, string> getData, CallBack onSuccess, CallBack onError)
        {
            string resultURL = URL;
            resultURL += "?";
            resultURL += getData.ToUrlData();
            HttpWebRequest request = WebRequest.CreateHttp(new Uri(resultURL));//создаем запрос
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; ru; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13 ( .NET CLR 3.5.30729)";
            request.CookieContainer = this.cookie;
            request.BeginGetResponse(new AsyncCallback(asynchronousResult =>
            {
                HttpWebRequest requestState = (HttpWebRequest)asynchronousResult.AsyncState;
                try
                {
                    HttpWebResponse response = (HttpWebResponse)requestState.EndGetResponse(asynchronousResult);
                    Stream streamResponse = response.GetResponseStream();
                    StreamReader streamRead = new StreamReader(streamResponse);
                    string responseString = streamRead.ReadToEnd();
                    streamResponse.Close();
                    streamRead.Close();
                    response.Close();
                    onSuccess(responseString);
                }
                catch (WebException e)
                {
                    onError(e);
                }
            }), request);
        }

        //private bool CheckAccessToken

        private string FindWithRegex(string SourceText, string RegExString)
        {
            Regex regex = new Regex(RegExString);
            Match match = regex.Match(SourceText);
            return match.Groups[1].Value;
        }

        /*private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")//если поля не пустые
            {
                string email = textBox1.Text;
                string pass = textBox2.Text;
                this.cookie.Add(new Uri("http://vk.com/"), new Cookie("remixlang", "3"));

                string URL1 = "https://oauth.vk.com/authorize";
                Dictionary<string, string> sendData1 = new Dictionary<string, string>();
                sendData1.Add("client_id", "3070837");
                sendData1.Add("scope", "notify,friends,photos,audio,video,docs,notes,pages,status,offers,questions,wall,groups,messages,notifications,stats,ads,offline");
                //sendData1.Add("scope", "2015231");
                sendData1.Add("redirect_uri", "http://oauth.vk.com/blank.html");
                sendData1.Add("display", "wap");
                sendData1.Add("response_type", "token");
                this.GetQuery(URL1, sendData1, result1 =>
                {
                    var responseString1 = (string)result1;
                    string URL2 = this.FindWithRegex(responseString1, "<form method=\"POST\" action=\"([a-z0-9:/?&=.]+)\"");
                    //string URL2 = this.FindWithRegex(responseString1, "<form method=\"POST\" id=\"login_submit\" action=\"([a-z0-9:/?&=.]+)\"");
                    string q = this.FindWithRegex(responseString1, "<input type=\"hidden\" name=\"q\" value=\"([0-9]+)\"");
                    string from_host = this.FindWithRegex(responseString1, "<input type=\"hidden\" name=\"from_host\" value=\"([A-Za-z0-9.]+)\"");
                    string from_protocol = this.FindWithRegex(responseString1, "<input type=\"hidden\" name=\"from_protocol\" value=\"([A-Za-z0-9]+)\"");
                    string ip_h = this.FindWithRegex(responseString1, "<input type=\"hidden\" name=\"ip_h\" value=\"([a-z0-9]+)\"");
                    string to = this.FindWithRegex(responseString1, "<input type=\"hidden\" name=\"to\" value=\"([A-Za-z0-9\\D]+)\"");
                    Dictionary<string, string> sendData2 = new Dictionary<string, string>();
                    sendData2.Add("q", q);
                    sendData2.Add("from_host", from_host);
                    sendData2.Add("from_protocol", from_protocol);
                    sendData2.Add("ip_h", ip_h);
                    sendData2.Add("to", to);
                    sendData2.Add("email", email);
                    sendData2.Add("pass", pass);
                    this.PostQuery(URL2, sendData2, result2 =>
                    {
                        var responseString2 = (string)result2;
                        //Deployment.Current.Dispatcher.BeginInvoke( () => 
                        //    { //your ui update code
                        //        WebBrowser1.NavigateToString(responseString2);
                        //    } );
                        string URL3 = this.FindWithRegex(responseString2, "<form method=\"POST\" name=\"login\" id=\"quick_login_form\" action=\"([a-z0-9:/?&=.]+)\"");
                        string act2 = this.FindWithRegex(responseString2, "<input type=\"hidden\" name=\"act\" value=\"([a-z]+)\"");
                        string q2 = this.FindWithRegex(responseString2, "<input type=\"hidden\" name=\"q\" value=\"([0-9]+)\"");
                        string al_frame2 = this.FindWithRegex(responseString2, "<input type=\"hidden\" name=\"al_frame\" value=\"([0-9]+)\"");
                        string expire2 = "";
                        string captcha_sid2 = "";
                        string captcha_key2 = "";
                        string from_host2 = "vk.com";
                        string from_protocol2 = "http";
                        string ip_h2 = this.FindWithRegex(responseString2, "<input type=\"hidden\" name=\"ip_h\" value=\"([a-z0-9]+)\"");
                        Dictionary<string, string> sendData3 = new Dictionary<string, string>();
                        sendData3.Add("act", act2);
                        sendData3.Add("q", q2);
                        sendData3.Add("al_frame", al_frame2);
                        sendData3.Add("expire", expire2);
                        sendData3.Add("captcha_sid", captcha_sid2);
                        sendData3.Add("captcha_key", captcha_key2);
                        sendData3.Add("from_host", from_host2);
                        sendData3.Add("from_protocol", from_protocol2);
                        sendData3.Add("ip_h", ip_h2);
                        sendData3.Add("email", email);
                        sendData3.Add("pass", pass);
                        this.PostQuery(URL3, sendData3, result3 =>
                        {
                            var responseString3 = (string)result3;
                            string sid = this.FindWithRegex(responseString3, "\'sid\', \'([a-z0-9]+)\'");
                            string viewer_id = this.FindWithRegex(responseString3, "parent.onLoginDone[(]\'/([a-z0-9]+)\'");
                            string URL4 = "https://oauth.vk.com/access_token";
                            Dictionary<string, string> sendData4 = new Dictionary<string, string>();
                            sendData4.Add("code", sid);
                            sendData4.Add("client_id", "3070837");
                            sendData4.Add("client_secret", "VykBAOy47loHDoOq9L54");
                            //sendData4.Add("uid", "ravikwow");
                            //sendData4.Add("api_id", "2400039");
                            //sendData4.Add("method", "getUserInfo");
                            //sendData4.Add("format", "xml");
                            //sendData4.Add("v", "2.0");
                            //string sig = sendData4.getSIG("ravikwow", "jMolbs7A50AVXAk7VSJm");
                            //sendData4.Add("sid", sid);
                            //sendData4.Add("sig", sig);
                            this.PostQuery(URL4, sendData4, result4 =>
                            {
                                var responseString4 = (string)result4;
                                var tmp = "";
                            });
                        });
                    });
                });
            }
            else { MessageBox.Show("Введите логин/пароль"); }
        }*/

        public event EventHandler NewsArrived;

        public void GetAccessToLongPoll()
        {
            string key, server, ts;
            var url = "https://api.vk.com/method/messages.getLongPollServer";
            var sendData = new Dictionary<string, string>
                               {
                                   {"access_token", this.access_token}
                               };

            this.GetQuery(url, sendData, res =>
                                                {
                                                    var responseString = (string)res;
                                                    var obj = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                                                    var response = obj["response"].ToString();
                                                    var decodedResponse = Newtonsoft.Json.Linq.JObject.Parse(response);
                                                    key = decodedResponse["key"].ToString();
                                                    ts = decodedResponse["ts"].ToString();
                                                    server = decodedResponse["server"].ToString();
                                                    this.BeginReceiving(server, key, ts);
                                                }, res =>
                                                       {
                                                           // DO_SOMETHING
                                                       });
        }

        public void BeginReceiving(string server, string key, string ts)
        {
            var url = "http://" + server;
            var sendData = new Dictionary<string, string>
                               {
                                   {"act", "a_check"},
                                   {"key", key},
                                   {"ts", ts},
                                   {"wait", "10"},
                                   {"mode", "2"}
                               };

            this.GetQuery(url, sendData, res =>
                                                   {
                                                       var decodedResponse = Newtonsoft.Json.Linq.JObject.Parse(res.ToString());
                                                       var j = decodedResponse["updates"];
                                                       NewsArrived.Invoke(j);
                                                       this.BeginReceiving(server, key, decodedResponse["ts"].ToString());
                                                   },
                                                    res =>
                                                    {
                                                        // DO_SOMETHING
                                                    });
        }

    }
}
