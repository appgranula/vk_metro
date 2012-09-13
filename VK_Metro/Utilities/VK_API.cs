using System.Windows.Controls;
using System.Windows.Media.Imaging;
using VK_Metro.Utilities;

namespace VK_Metro
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Windows.Threading;
    using Newtonsoft.Json;
    using VK_Metro.Models;

    public delegate void UpdatesArrivedEventHandler(UpdateModel updates);

    public delegate void CallBack(object param);

    public class VK_API
    {
        public string captchaImageAddress;

        private string access_token;
        private string user_id;
        private CookieContainer cookie;
        private bool _connect;
        private Dictionary<string, string> lastRequest;
        private string lastSid;
        private Dictionary<string, bool> receivedMessageHistory;

        public VK_API()
        {
            this.cookie = new CookieContainer();
            this._connect = false;
            this.receivedMessageHistory = new Dictionary<string, bool>();
        }

        public event UpdatesArrivedEventHandler UpdatesArrived;

        public bool connected
        {
            get { return _connect; }
        }

        public void Connect(string email, string password, CallBack onSuccess, CallBack onError)
        {
            string URL = "https://api.vk.com/oauth/token";
            var sendData = new Dictionary<string, string>();
            sendData.Add("grant_type", "password");
            sendData.Add("client_id", "3070837");
            sendData.Add("client_secret", "VykBAOy47loHDoOq9L54");
            sendData.Add("scope",
                         "notify,friends,photos,audio,video,docs,notes,pages,status,offers,questions,wall,groups,messages,notifications,stats,ads,offline");
            sendData.Add("username", email);
            sendData.Add("password", password);
            this.GetQuery(URL, sendData,
                          result =>
                              {
                                  var responseString = (string) result;
                                  Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                                  this.access_token = obj["access_token"].ToString();
                                  this.user_id = obj["user_id"].ToString();
                                  this._connect = true;
                                  IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                                  if (settings.Contains("access_token"))
                                      settings["access_token"] = access_token;
                                  else
                                      settings.Add("access_token", access_token);
                                  if (settings.Contains("user_id"))
                                      settings["user_id"] = user_id;
                                  else
                                      settings.Add("user_id", user_id);
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
                sendData.Add("fields", "uid,first_name,last_name,nickname,screen_name,sex,bdate,timezone,photo,photo_big,online");
                sendData.Add("order", "hint");
                this.GetQuery(URL, sendData, result =>
                                                 {
                                                     var responseString = (string) result;
                                                     Newtonsoft.Json.Linq.JObject obj =
                                                         Newtonsoft.Json.Linq.JObject.Parse(responseString);
                                                     if (obj["response"] != null)
                                                     {
                                                         VKFriendModel[] friends =
                                                             obj["response"].ToObject<VKFriendModel[]>();
                                                         var n = friends.Length <= 5 ? friends.Length : 5;
                                                         for (var i = 0; i < n; i++)
                                                         {
                                                             if (friends[i] != null)
                                                                 friends[i].hint = i + 1;
                                                         }
                                                         onSuccess(friends);
                                                     }
                                                     else
                                                     {
                                                         onError(new object());
                                                     }
                                                 }, error =>
                                                        {
                                                            onError(error);
                                                        });
            }
        }

        public void GetUser(string uid, CallBack onSuccess, CallBack onError)
        {
            if (!this.connected) return;
            var access_token = this.access_token;
            string URL = "https://api.vk.com/method/users.get";
            Dictionary<string, string> sendData = new Dictionary<string, string>();
            sendData.Add("access_token", access_token);
            sendData.Add("uids", uid);
            sendData.Add("fields", "uid,first_name,last_name,nickname,screen_name,sex,bdate,timezone,photo,photo_medium,photo_big,online");
            this.PostQuery(URL, sendData, result =>
                                             {
                                                 var responseString = (string) result;
                                                 Newtonsoft.Json.Linq.JObject obj =
                                                     Newtonsoft.Json.Linq.JObject.Parse(responseString);
                                                 if (obj["response"] != null)
                                                 {
                                                     VKFriendModel[] friends =
                                                         obj["response"].ToObject<VKFriendModel[]>();
                                                     if (friends.Length == 1)
                                                     {
                                                         onSuccess(friends[0]);
                                                     }
                                                     else
                                                     {
                                                         onSuccess(friends);
                                                     }
                                                 }
                                                 else
                                                 {
                                                     onError(new object());
                                                 }
                                             }, error =>
                                                    {
                                                        onError(error);
                                                    });
        }

        public void SignUp(string nummberPhone, string firstName, string lastName, CallBack onSuccess, CallBack onError)
        {
            var sendData = new Dictionary<string, string>
                               {
                                   {"phone", nummberPhone},
                                   {"first_name", firstName},
                                   {"last_name", lastName},
                                   {"client_id", "3070837"},
                                   {"client_secret", "VykBAOy47loHDoOq9L54"}
                               };
            this.SignUpQuerry(sendData, onSuccess, onError);
        }

        public void RepeatLastRequestWithCaptcha(string captcha, CallBack onSuccess, CallBack onError)
        {
            if (!this.lastRequest.ContainsKey("captcha_key"))
            {
                this.lastRequest.Add("captcha_key", captcha);
            }
            else
            {
                this.lastRequest["captcha_key"] = captcha;
            }

            this.SignUpQuerry(this.lastRequest, onSuccess, onError);
        }

        public void IDidntReceiveSMS(CallBack onSuccess, CallBack onError)
        {
            if (!this.lastRequest.ContainsKey("sid"))
            {
                this.lastRequest.Add("sid", this.lastSid);
            }
            else
            {
                this.lastRequest["sid"] = this.lastSid;
            }

            this.SignUpQuerry(this.lastRequest, onSuccess, onError);
        }

        private void SignUpQuerry(Dictionary<string, string> sendData, CallBack onSuccess, CallBack onError)
        {
            var url = "https://api.vk.com/method/auth.signup";
            this.GetQuery(
                url,
                sendData,
                res =>
                    {
                        var responseString = (string) res;
                        var obj = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                        this.lastRequest = sendData;
                        if (obj["response"] != null)
                        {
                            var response = obj["response"]["sid"].ToString();
                            this.lastSid = response;
                            onSuccess(response);
                        }
                        else if (obj["error"] != null)
                        {
                            var code = obj["error"]["error_code"].ToString();
                            if (code == "14")
                            {
                                var captchaSid = obj["error"]["captcha_sid"].ToString();
                                this.captchaImageAddress = obj["error"]["captcha_img"].ToString();

                                if (!sendData.ContainsKey("captcha_sid"))
                                {
                                    sendData.Add("captcha_sid", captchaSid);
                                }
                                else
                                {
                                    sendData["captcha_sid"] = captchaSid;
                                }

                                onSuccess("captcha");
                                return;
                            }

                            if (code == "100")
                            {
                                var errorMessage = "Ошибка в веденных данных";
                                onError(errorMessage);
                                return;
                            }
                        }
                    },
                res =>
                    {
                        // DO_SOMETHING
                    });
        }

        public void ConfirmSignUp(string code, string password, CallBack onSuccess, CallBack onError)
        {
            var url = "https://api.vk.com/method/auth.confirm";
            var sendData = new Dictionary<string, string>
                               {
                                   {"phone", this.lastRequest["phone"]},
                                   {"code", code},
                                   {"password", password},
                                   {"client_id", "3070837"},
                                   {"client_secret", "VykBAOy47loHDoOq9L54"},
                                   {"test_mode", "1"}
                               };
            this.GetQuery(
                url,
                sendData,
                result =>
                    {
                        var responseString = (string) result;
                        var obj = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                        if (obj["response"] != null)
                        {
                            var response = obj["response"]["success"].ToString();
                            var uid = obj["response"]["uid"].ToString();
                            onSuccess(response);
                        }
                        else if (obj["error"] != null)
                        {
                            // DO_SOMETHING
                        }

                        onSuccess(responseString);
                    },
                result =>
                    {
                        onError(result);
                    });
        }

        public void CheckPhone(string phone, CallBack onSuccess, CallBack onError)
        {
            var url = "https://api.vk.com/method/auth.checkPhone";
            var sendData = new Dictionary<string, string>
                               {
                                   {"phone", phone},
                                   {"client_id", "3070837"},
                                   {"client_secret", "VykBAOy47loHDoOq9L54"},
                               };
            this.GetQuery(
                url,
                sendData,
                result =>
                    {
                        var responseString = (string) result;
                        var obj = Newtonsoft.Json.Linq.JObject.Parse(responseString);

                        if (obj["response"] != null)
                        {
                            onSuccess(obj["response"]);
                            return;
                        }
                        else if (obj["error"] != null)
                        {
                            var errorMessage = string.Empty;
                            switch (short.Parse(obj["error"]["error_code"].ToString()))
                            {
                                case 100:
                                    {
                                        errorMessage = "Введенный вами номер неверен";
                                        break;
                                    }

                                case 1003:
                                    {
                                        errorMessage =
                                            "User already invited: message already sended, you can resend message in 300 seconds";
                                        break;
                                    }

                                case 1004:
                                    {
                                        errorMessage = "This phone used by another user";
                                        break;
                                    }

                                case 1112:
                                    {
                                        errorMessage = "Processing.. Try later";
                                        break;
                                    }
                            }

                            onError(errorMessage);
                            return;
                        }
                    },
                result =>
                    {
                        onError(result);
                    });
        }

        public void CheckContacts(string phones, CallBack onSuccess, CallBack onError)
        {
            var sendData = new Dictionary<string, string>
                               {
                                   {"access_token", this.access_token},
                                   {"phones", phones},
                                   {
                                       "fields",
                                       "uid,first_name,last_name,nickname,screen_name,sex,bdate,timezone,photo,online"
                                       }
                               };

            var url = "https://api.vk.com/method/friends.getByPhones";
            this.GetQuery(
                url,
                sendData,
                result =>
                    {
                        var responseString = (string) result;
                        var obj = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                        if (obj["error"] != null)
                        {
                            var errorArray = new Dictionary<string, string>
                                                 {
                                                     {"error_code", obj["error"]["error_code"].ToString()},
                                                     {"error_msg", obj["error"]["error_msg"].ToString()}
                                                 };
                            onError(errorArray);
                            return;
                        }

                        var contacts = obj["response"];
                        var userList = new List<Dictionary<string, string>>();
                        foreach (var i in contacts)
                        {
                            userList.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(i.ToString()));
                        }

                        onSuccess(userList);
                    },
                result =>
                    {
                        onError(result);
                    });
        }

        public void GetDialogs(CallBack onSuccess, CallBack onError)
        {
            if (!this.connected) return;
            var access_token = this.access_token;
            string URL = "https://api.vk.com/method/messages.getDialogs";
            Dictionary<string, string> sendData = new Dictionary<string, string>();
            sendData.Add("access_token", access_token);
            this.GetQuery(URL, sendData, result =>
                                             {
                                                 var responseString = (string) result;
                                                 Newtonsoft.Json.Linq.JObject obj =
                                                     Newtonsoft.Json.Linq.JObject.Parse(responseString);
                                                 if (obj["response"] != null)
                                                 {
                                                     obj["response"].First.Remove();
                                                     VKMessageModel[] messages =
                                                         obj["response"].ToObject<VKMessageModel[]>();
                                                     onSuccess(messages);
                                                 }
                                                 else
                                                 {
                                                     onError(new object());
                                                 }
                                             }, onError);
        }

        public void GetMessage(string mid, CallBack onSuccess, CallBack onError)
        {
            if (!this.connected) return;
            var access_token = this.access_token;
            string URL = "https://api.vk.com/method/messages.getById";
            Dictionary<string, string> sendData = new Dictionary<string, string>();
            sendData.Add("access_token", access_token);
            sendData.Add("mid", mid);
            this.GetQuery(URL, sendData, result =>
            {
                var responseString = (string)result;
                Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                if (obj["response"] != null)
                {
                    obj["response"].First.Remove();
                    VKMessageModel[] messages = obj["response"].ToObject<VKMessageModel[]>();
                    onSuccess(messages);
                }
                else
                {
                    onError(new object());
                }
            }, onError);
        }
        public void GetMessagesWithFilter(string query, CallBack onSuccess, CallBack onError) 
        {
            if (!this.connected) return;
            var access_token = this.access_token;
            string URL = "https://api.vk.com/method/messages.search";
            Dictionary<string, string> sendData = new Dictionary<string, string>();
            sendData.Add("access_token", access_token);
            sendData.Add("q",query);
            this.GetQuery(URL, sendData, result =>
            {
                var responseString = (string)result;
                Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                if (obj["response"] != null)
                {
                    obj["response"].First.Remove();
                    VKMessageModel[] messages = obj["response"].ToObject<VKMessageModel[]>();
                    onSuccess(messages);
                }
                else
                {
                    onError(new object());
                }
            }, onError);
        }
        public void GetChatsWithFilter(string query, CallBack onSuccess, CallBack onError) 
        {
            if (!this.connected) return;
            var access_token = this.access_token;
            string URL = "https://api.vk.com/method/messages.searchDialogs";
            Dictionary<string, string> sendData = new Dictionary<string, string>();
            sendData.Add("access_token", access_token);
            sendData.Add("q", query);
            this.GetQuery(URL, sendData, result =>
            {
                var responseString = (string)result;
                Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                if (obj["response"] != null)
                {
                    //obj["response"].First.Remove();
                    VKChatModel[] chats = obj["response"].ToObject<VKChatModel[]>();
                    onSuccess(chats);
                }
                else
                {
                    onError(new object());
                }
            }, onError);
        }
        public void SendMessage(string uid, string message, CallBack onSuccess, CallBack onError)
        {
            if (!this.connected) return;
            var access_token = this.access_token;
            string URL = "https://api.vk.com/method/messages.send";
            Dictionary<string, string> sendData = new Dictionary<string, string>();
            sendData.Add("access_token", access_token);
            sendData.Add("uid", uid);
            sendData.Add("message", message);
            this.GetQuery(URL, sendData, result =>
            {
                var responseString = (string)result;
                Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                if (obj["response"] != null)
                {
                    string mid = obj["response"].ToObject<string>();
                    onSuccess(mid);
                }
                else
                {
                    onError(new object());
                }
            }, onError);
        }

        private void PostQuery(string URL, Dictionary<string, string> postData, CallBack onSuccess, CallBack onError)
        {
            //string boundary = "----------" + DateTime.Now.Ticks.ToString();
            HttpWebRequest request = WebRequest.CreateHttp(new Uri(URL)); //создаем запрос
            request.ContentType = "application/x-www-form-urlencoded";
            //request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            //request.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/msword, application/vnd.ms-excel, application/vnd.ms-powerpoint, */*";
            request.UserAgent =
                "Mozilla/5.0 (Windows; U; Windows NT 6.0; ru; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13 ( .NET CLR 3.5.30729)";
            request.Method = "POST";
            request.Headers["Accept-Charset"] = "windows-1251,utf-8;q=0.7,*;q=0.3";
            request.Headers["Accept-Language"] = "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4";
            request.CookieContainer = this.cookie;
            Thread thead = new Thread(delegate()
            {
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
            });
            thead.Start();
        }

        private void GetQuery(string URL, Dictionary<string, string> getData, CallBack onSuccess, CallBack onError)
        {
            string resultURL = URL;
            resultURL += "?";
            resultURL += getData.ToUrlData();
            HttpWebRequest request = WebRequest.CreateHttp(new Uri(resultURL)); //создаем запрос
            request.UserAgent =
                "Mozilla/5.0 (Windows; U; Windows NT 6.0; ru; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13 ( .NET CLR 3.5.30729)";
            request.CookieContainer = this.cookie;
            Thread thead = new Thread(delegate()
            {
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
            });
            thead.Start();
        }

        public void CheckOldSession(CallBack onSuccess, CallBack onError)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("access_token") && settings.Contains("user_id"))
            {
                var access_token = (string) settings["access_token"];
                var user_id = (string) settings["user_id"];
                string URL = "https://api.vk.com/method/messages.get";
                Dictionary<string, string> sendData = new Dictionary<string, string>();
                sendData.Add("access_token", access_token);
                sendData.Add("count", "1");
                this.GetQuery(URL, sendData, result =>
                                                 {
                                                     var responseString = (string) result;
                                                     Newtonsoft.Json.Linq.JObject obj =
                                                         Newtonsoft.Json.Linq.JObject.Parse(responseString);
                                                     if (obj["response"] != null)
                                                     {
                                                         this.access_token = access_token;
                                                         this.user_id = user_id;
                                                         this._connect = true;
                                                         onSuccess(new object());
                                                     }
                                                     else
                                                     {
                                                         onError(new object());
                                                     }
                                                 }, error =>
                                                        {
                                                            onError(error);
                                                        });
            }
            else
            {
                onError(new object());
            }
        }

        public void Disconnect()
        {
            this._connect = false;
            this.access_token = null;
            this.user_id = null;
            this.cookie = new CookieContainer();
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("access_token"))
                settings.Remove("access_token");
            if (settings.Contains("user_id"))
                settings.Remove("user_id");
        }

        public void ConnectToLongPoll()
        {
            if (!this.connected)
            {
                return;
            }

            var url = "https://api.vk.com/method/messages.getLongPollServer";
            var sendData = new Dictionary<string, string>
                               {
                                   {"access_token", this.access_token}
                               };

            this.GetQuery(
                url,
                sendData,
                res =>
                    {
                        var responseString = (string) res;
                        var obj = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                        var response = obj["response"].ToString();
                        var decodedResponse = Newtonsoft.Json.Linq.JObject.Parse(response);
                        var key = decodedResponse["key"].ToString();
                        var ts = decodedResponse["ts"].ToString();
                        var server = decodedResponse["server"].ToString();
                        this.BeginReceivingFromLongPoll(server, key, ts);
                    },
                res =>
                    {
                        // DO_SOMETHING
                    });
        }

        private void BeginReceivingFromLongPoll(string server, string key, string ts)
        {
            var url = "http://" + server;
            var sendData = new Dictionary<string, string>
                               {
                                   {"act", "a_check"},
                                   {"key", key},
                                   {"ts", ts},
                                   {"wait", "25"},
                                   {"mode", "2"}
                               };

            this.GetQuery(
                url,
                sendData,
                res =>
                    {
                        // Uncomment for test
                        //res = "{\"ts\":1753641594,\"updates\":[[4,3,561,670025,1346214060,\" ... \",\"kj\",{\"attach1_type\":\"photo\",\"attach1\":\"670025_289067230\"}], [ 9, -23498, 1 ], [ 62, -23498, 123]]}";
                        //res = "{\"ts\":1753641594,\"updates\":[[1,1111222,768]]}";

                        var decodedResponse = Newtonsoft.Json.Linq.JObject.Parse(res.ToString());
                        var j = decodedResponse["updates"];

                        if (j != null)
                        {
                            var convertedResponse = decodedResponse.ToObject<UpdateModel>();

                            // If json response contains dictionary (i.e. attaches), 
                            // find it and convert in Dictionary<string, string> type
                            foreach (var i in convertedResponse.updates)
                            {
                                for (int count = 0; count < i.Length; count++)
                                {
                                    if (i[count] as Newtonsoft.Json.Linq.JObject == null) continue;
                                    var values =
                                        JsonConvert.DeserializeObject<Dictionary<string, string>>(i[count].ToString());
                                    i[count] = values;
                                }
                            }

                            UpdatesArrived.Invoke(convertedResponse);
                            this.BeginReceivingFromLongPoll(server, key, decodedResponse["ts"].ToString());
                        }
                        else
                        {
                            this.ConnectToLongPoll();
                        }

                    },
                res =>
                    {
                        // DO_SOMETHING
                    });
        }

        private void BeginCheckForFriendsRequests(CallBack onSuccess, CallBack onError)
        {
            if (!this.connected)
            {
                return;
            }

            var url = "https://api.vk.com/method/friends.getRequests";
            var sendData = new Dictionary<string, string>
                               {
                                   {"access_token", this.access_token}
                               };
            this.GetQuery(
                url,
                sendData,
                res =>
                    {
                        //res = "{\"response\":[2399082,17347602,775654,670025,17347602,2399082,17347602,775654,670025,17347602,2399082,17347602,775654,670025,17347602]}";
                        //res = "{\"response\":[670025]}";
                        var decodedResponse = Newtonsoft.Json.Linq.JObject.Parse(res.ToString());
                        var requests = decodedResponse["response"];
                        var convertedRequests = JsonConvert.DeserializeObject<List<int>>(requests.ToString());

                        onSuccess(convertedRequests);
                    },
                res =>
                    {
                        onError(res);
                    }
                );
        }

        public void StartCheckiingFriendsRequests(CallBack onSuccess, CallBack onError)
        {
            var dt = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 30, 0)};
            dt.Tick += (s, e) => this.BeginCheckForFriendsRequests(onSuccess, onError);
            // Execute it right now
            this.BeginCheckForFriendsRequests(onSuccess, onError);
            // and every N seconds
            dt.Start();
        }

        public void CheckForPossibleFriends(CallBack onSuccess, CallBack onError)
        {
            if (!this.connected)
            {
                return;
            }

            var url = "https://api.vk.com/method/friends.getSuggestions";
            var sendData = new Dictionary<string, string>
                               {
                                   {"access_token", this.access_token},
                                   {"filter", "mutual"},
                                   {"count", "50"}
                               };
            this.GetQuery(
            url,
            sendData,
            res =>
            {
                   
                var decodedResponse = Newtonsoft.Json.Linq.JObject.Parse(res.ToString());
                var requests = decodedResponse["response"];
                //var userList = new List<Dictionary<string, string>>();
                if (requests == null)
                {
                    onError(new object());
                }
                VKFriendModel[] userList = requests.ToObject<VKFriendModel[]>();
                /*foreach (var i in requests)
                {
                userList.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(i.ToString()));
                }*/

                onSuccess(userList);
            },
               res =>
               {
                   onError(res);
               }
               );
        }

        public void AddVkFriend(string uid, CallBack onSuccess, CallBack onError)
        {
            if (!this.connected)
            {
                return;
            }

            var url = "https://api.vk.com/method/friends.add";
            var sendData = new Dictionary<string, string>
                               {
                                   { "access_token", this.access_token },
                                   { "uid", uid }
                               };
            this.GetQuery(
                url,
                sendData,
                res =>
                    {
                        var decodedResponse = Newtonsoft.Json.Linq.JObject.Parse(res.ToString());
                        var answer = decodedResponse["response"];
                        if (answer == null)
                        {
                            onError(new object());
                            return;
                        }

                        onSuccess(answer.ToString());
                    },
                res =>
                    {
                        onError(new object());
                    });
        }

        public void DeleteVkFriend(string uid, CallBack onSuccess, CallBack onError)
        {
            if (!this.connected)
            {
                return;
            }

            var url = "https://api.vk.com/method/friends.delete";
            var sendData = new Dictionary<string, string>
                               {
                                   { "access_token", this.access_token },
                                   { "uid", uid }
                               };
            this.GetQuery(
                url,
                sendData,
                res =>
                {
                    var decodedResponse = Newtonsoft.Json.Linq.JObject.Parse(res.ToString());
                    var answer = decodedResponse["response"];
                    if (answer == null)
                    {
                            onError(new object());
                        return;
                    }

                    onSuccess(answer.ToString());
                },
                res =>
                {
                    onError(new object());
                });
        }

        public void MarkAsRead(string mids, CallBack onSuccess, CallBack onError)
        {
            if (!this.connected)
            {
                return;
            }

            var url = "https://api.vk.com/method/messages.markAsRead";
            var sendData = new Dictionary<string, string>
                               {
                                   { "access_token", this.access_token },
                                   { "mids", mids }
                               };
            this.GetQuery(
                url,
                sendData,
                res =>
                {
                    var decodedResponse = Newtonsoft.Json.Linq.JObject.Parse(res.ToString());
                    var answer = decodedResponse["response"];
                    if (answer == null)
                    {
                        onError(new object());
                        return;
                    }

                    onSuccess(answer.ToString());
                },
                res =>
                {
                    onError(new object());
                });
        }

        public void GetMessagesUploadServer(CallBack onSuccess, CallBack onError)
        {
            if (!this.connected)
            {
                return;
            }

            var url = "https://api.vk.com/method/photos.getMessagesUploadServer";
            var sendData = new Dictionary<string, string>
                               {
                                    { "access_token", this.access_token }
                               };
            this.GetQuery(
                url,
                sendData,
                res =>
                {
                    var decodedResponse = Newtonsoft.Json.Linq.JObject.Parse(res.ToString());
                    var answer = decodedResponse["response"];
                    if (answer == null)
                    {
                        onError(new object());
                        return;
                    }

                    onSuccess(answer["upload_url"].ToString());
                },
                res =>
                {
                    onError(new object());
                });
        }

        public void SaveMessagesPhoto(string server, string photo, string hash, CallBack onSuccess, CallBack onError)
        {
            if (!this.connected)
            {
                return;
            }

            var url = "https://api.vk.com/method/photos.saveMessagesPhoto";
            var sendData = new Dictionary<string, string>
            {
                { "access_token", this.access_token },
                {"server", server},
                {"photo", photo},
                {"hash", hash}
            };

            this.GetQuery(
                url,
                sendData,
                res =>
                {
                    var decodedResponse = Newtonsoft.Json.Linq.JObject.Parse(res.ToString());
                    var answer = decodedResponse["response"];
                    if (answer == null)
                    {
                        onError(new object());
                        return;
                    }
                    var idList = new List<string>();
                    foreach (var i in answer)
                    {
                        idList.Add((JsonConvert.DeserializeObject<Dictionary<string, string>>(i.ToString()))["id"].ToString());
                    }
                    //var id = Newtonsoft.Json.Linq.JObject.Parse(answer.ToString());
                    onSuccess(idList);
                },
                res =>
                {
                    onError(new object());
                });
        }

        public void UploadPhotoToServer(string server, byte[] photo, CallBack onSuccess, CallBack onError)
        {
            var postData = new Dictionary<string, object>()
                                                {
                                                    {"access_token", this.access_token },
                                                    {"photo", photo}
                                                    //You can add other parameters here
                                                };
            var postToServer = new PostSubmitter { url = server, parameters = postData };
            postToServer.PostResponseEvent += (sender, args) => this.SaveMessagesPhoto(
                args.Server,
                args.Photo,
                args.Hash,
                result =>
                {
                    
                    int i = 8;
                    onSuccess(result.ToString());
                },
                result =>
                    {
                        int i = 9;
                    });
            postToServer.Submit();
        }
    }
}
