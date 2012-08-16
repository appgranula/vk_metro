﻿using System;
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
                                friends[i].hint = i+1;
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

        public void SignUp(string NummberPhone, string FirstName, string LastName, CallBack onSuccess, CallBack onError)
        {
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
    }

}
