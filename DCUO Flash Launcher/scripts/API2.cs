﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Diagnostics;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using System.Collections;

namespace FlashLauncher
{
    internal static class API2
    {
        /// <summary>
        /// Client Object for interacting with the web
        /// </summary>
        internal static HttpClient Client { get; set; }

        /// <summary>
        /// Object for storing cookies
        /// </summary>
        internal static CookieContainer Cookies { get; set; }
        internal static HttpClientHandler Handler { get; set; }

        /// <summary>
        /// Object for storing web responses
        /// </summary>
        internal static HttpResponseMessage Response { get; set; }

        /// <summary>
        /// Object for storing a copy of the provided user account
        /// </summary>
        internal static Account UserAccount { get; set; }

        /// <summary>
        /// If true: the provided account has valid credentials
        /// </summary>
        internal static bool IsLoggedIn { get; set; }

        /// <summary>
        /// launch arguments for the game
        /// </summary>
        internal static string LaunchArgs { get; set; }

        /// <summary>
        /// Path to launchmefirst executable
        /// </summary>
        internal static string dcuoLaunchmefirstPath { get; set; }

        /// <summary>
        /// Storage container for the current playsession
        /// </summary>
        internal static PlaySession PS { get; set; }


        /// <summary>
        /// Set username and passwort that is used for API access
        /// </summary>
        /// <param name="account"></param>
        public static void SetAccount(Account account)
        {
            UserAccount = account;
        }

        /// <summary>
        /// Use this function to try to login with the provided user account
        /// </summary>
        public static void Login()
        {
            if (UserAccount == null)
            {
                throw new Exception("you need to provide a user account befor you can login");
            }
            // get session Cookie
            Debug.WriteLine("[ API:Login ] Connecting to: " + URL.Live);
            Response = Client.GetAsync(URL.Live).Result;
            Uri uri = new Uri(URL.Live);
            IEnumerable<Cookie> responseCookies = Cookies.GetCookies(uri).Cast<Cookie>();
            foreach (Cookie cookie in responseCookies)
            {
                Debug.WriteLine("[ API:Login ] set cookie: " + cookie.Name + ": " + cookie.Value);
            }

            Debug.WriteLine("[ API:Login ] adding Username " + UserAccount.Username + " and password to cookie");
            FormUrlEncodedContent content = new(new[]
            {
                new KeyValuePair<string, string>("username", UserAccount.Username),
                new KeyValuePair<string, string>("password", UserAccount.Password),
            });
            Debug.WriteLine("[ API:Login ] posting data to: " + URL.Login);
            HttpResponseMessage httpResponseMessage = Client.PostAsync(URL.Login, content).Result;
            Response = httpResponseMessage;
            string _data = Response.Content.ReadAsStringAsync().Result;
            Debug.WriteLine("[ API:Login ] post result: " + _data);

            Debug.WriteLine("[ API:Login ] getting current cookies...");
            uri = new Uri(URL.Login);
            responseCookies = Cookies.GetCookies(uri).Cast<Cookie>();
            foreach (Cookie cookie in responseCookies)
            {
                Debug.WriteLine("[ API:Login ] Cookie Login: " + cookie.Name + ": " + cookie.Value);
            }
            bool _ = CheckLogin(Response.Content.ReadAsStringAsync().Result);
        }

        /// <summary>
        /// Calls /get_play_session and gets the launch arguments for the game
        /// Login needs to be run first
        /// </summary>
        public static async Task<string> GetLaunchArgs()
        {
            if (!IsLoggedIn)
            {
                throw new Exception("you need to be logged in to use the function");
            }
            Debug.WriteLine("[ API:GetLaunchArgs] getting launch arguments...");
            Debug.WriteLine("[ API:GetLaunchArgs] connecting to: " + URL.GetPlaySession);

            HttpRequestMessage get = new HttpRequestMessage(HttpMethod.Get, URL.GetPlaySession);
            get.Content = new StringContent("application/json");
            HttpResponseMessage response = await Client.SendAsync(get);
            string result = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("[ API:GetLaunchArgs] Result: " + result);
            PS = JsonConvert.DeserializeObject<PlaySession>(result);


            Uri uri = new Uri(URL.GetPlaySession);
            IEnumerable<Cookie> responseCookies = Cookies.GetCookies(uri).Cast<Cookie>();
            foreach (Cookie cookie in responseCookies)
                Debug.WriteLine("[ API:GetLaunchArgs ] Cookie: " + cookie.Name + ": " + cookie.Value);
            Debug.WriteLine("[ API:GetLaunchArgs ] LaunchArgs: " + PS.LaunchArgs);
            return PS.LaunchArgs;
        }

        /// <summary>
        /// Checks if the Login was successfully and sets "IsLoggedIn" value to <see langword="true"/> or <see langword="false"/>
        /// </summary>
        /// <param name="jsession"></param>
        /// <returns><see langword="true"/> or <see langword="false"/></returns>
        public static bool CheckLogin(string jsession)
        {
            Debug.WriteLine("[ CheckLogin ] Jsession: " + jsession);
            PlaySession? session = new PlaySession();
            session = JsonConvert.DeserializeObject<PlaySession>(jsession);
            Debug.WriteLine("[ CheckLogin ] Username and Category: " + session.Username + " " + session.Category);
            if (session.Username == UserAccount.Username && session.Category == "SUCCESS")
            {
                IsLoggedIn = true;
                return true;
            }
            else
            {
                IsLoggedIn = false;
                return false;
            }
        }


        /// <summary>
        /// Launches the Game with provided arguments
        /// </summary>
        /// <param name="_LaunchArgs"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void LaunchGame(string _LaunchArgs)
        {
            string _argName = "_LaunchArgs";
            if (!String.IsNullOrEmpty(_LaunchArgs))
            {
                Process dcuo = new()
                {
                    StartInfo = new()
                    {
                        FileName = dcuoLaunchmefirstPath,
                        Arguments = _LaunchArgs,
                        UseShellExecute = false,
                        CreateNoWindow = false
                    }
                };
                Debug.WriteLine("[ OK:Launch ] launching game! Args: " + _LaunchArgs);
                dcuo.Start();
            }
            else
            {
                Debug.WriteLine("[ ERROR:Launch ] _LaunchArgs : " + _argName);
                throw new ArgumentNullException(_argName);
            }
        }
    }
}