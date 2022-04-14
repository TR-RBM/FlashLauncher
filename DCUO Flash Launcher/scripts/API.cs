using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Diagnostics;
using Newtonsoft.Json;

namespace FlashLauncher
{
    public class API
    {
        /// <summary>
        /// Client Object for interacting with the web
        /// </summary>
        private HttpClient Client { get; set; }

        /// <summary>
        /// Object for storing cookies
        /// </summary>
        private CookieContainer Cookies { get; set; }
        private HttpClientHandler Handler { get; set; }

        /// <summary>
        /// Object for storing web responses
        /// </summary>
        private HttpResponseMessage Response { get; set; }

        /// <summary>
        /// Object for storing a copy of the provided user account
        /// </summary>
        private Account UserAccount { get; set; }

        /// <summary>
        /// If true: the provided account has valid credentials
        /// </summary>
        public bool IsLoggedIn { get; set; }

        /// <summary>
        /// launch arguments for the game
        /// </summary>
        private string LaunchArgs { get; set; }
        public string dcuoLaunchmefirstPath { get; set; }

        /// <summary>
        /// API für interacting with the DCUO / Daybreak / Census API
        /// </summary>
        public API()
        {
            Cookies = new CookieContainer();
            Handler = new HttpClientHandler();
            Handler.CookieContainer = Cookies;
            Client = new HttpClient(Handler);
            IsLoggedIn = false;
            LaunchArgs = "";
            dcuoLaunchmefirstPath = new(@"C:\Users\Public\Daybreak Game Company\Installed Games\DC Universe Online\UNREAL3\BINARIES\WIN32\LAUNCHMEFIRST.EXE");
        }

        /// <summary>
        /// Set username and passwort that is used for API access
        /// </summary>
        /// <param name="account"></param>
        public void SetAccount(Account account)
        {
            UserAccount = account;
        }

        /// <summary>
        /// Use this function to try to login with the provided user account
        /// </summary>
        public async void Login()
        {
            if (UserAccount == null)
            {
                throw new Exception("you need to provide a user account befor you can login");
            }

            using (Handler)
            {
                using (Client)
                {
                    // get session Cookie
                    Response = Client.GetAsync("https://lpj.daybreakgames.com/dcuo/live/").Result;
                    Uri uri = new Uri("https://lpj.daybreakgames.com/dcuo/live/");
                    IEnumerable<Cookie> responseCookies = Cookies.GetCookies(uri).Cast<Cookie>();
                    foreach (Cookie cookie in responseCookies)
                    {
                        Debug.WriteLine("Cookie Session: " + cookie.Name + ": " + cookie.Value);
                    }

                    FormUrlEncodedContent content = new(new[]
                    {
                        new KeyValuePair<string, string>("username", UserAccount.Username),
                        new KeyValuePair<string, string>("password", UserAccount.Password),
                    });

                    HttpResponseMessage result = await Client.PostAsync("https://lpj.daybreakgames.com/dcuo/live/login", content);
                    //result.EnsureSuccessStatusCode();
                    Debug.WriteLine(await result.Content.ReadAsStringAsync());

                    uri = new Uri("https://lpj.daybreakgames.com/dcuo/live/login");
                    responseCookies = Cookies.GetCookies(uri).Cast<Cookie>();
                    foreach (Cookie cookie in responseCookies)
                    {
                        Debug.WriteLine("Cookie Login: " + cookie.Name + ": " + cookie.Value);
                    }
                    bool _ = CheckLogin(Response.Content.ReadAsStringAsync().Result);
                }
            }
        }

        /// <summary>
        /// Calls /get_play_session and gets the launch arguments for the game
        /// Login needs to be run first
        /// </summary>
        public void GetLaunchArgs()
        {
            if (!IsLoggedIn)
            {
                throw new Exception("you need to be logged in to use the function");
            }
            using (Handler)
            {
                using (Client)
                {
                    Response = Client.GetAsync("https://lpj.daybreakgames.com/dcuo/live/get_play_session").Result;
                    Uri uri = new Uri("https://lpj.daybreakgames.com/dcuo/live/get_play_session");
                    IEnumerable<Cookie> responseCookies = Cookies.GetCookies(uri).Cast<Cookie>();
                    foreach (Cookie cookie in responseCookies)
                        Debug.WriteLine("get_play_session Cookie: " + cookie.Name + ": " + cookie.Value);
                    Debug.WriteLine("get_play_session: " + Response.Content.ReadAsStringAsync().Result);
                    LaunchArgs = Response.Content.ReadAsStringAsync().Result;
                }
            }
        }

        /// <summary>
        /// Checks if the Login was successfully and sets "IsLoggedIn" value to <see langword="true"/> or <see langword="false"/>
        /// </summary>
        /// <param name="jsession"></param>
        /// <returns><see langword="true"/> or <see langword="false"/></returns>
        private bool CheckLogin(string jsession)
        {
            Debug.WriteLine("func(): CheckLogin: " + jsession);
            PlaySession? session = new PlaySession();
            session = JsonConvert.DeserializeObject<PlaySession>(jsession);
            Debug.WriteLine("func(): CheckLogin: " + session.username + " " + session.category);
            if (session.username == UserAccount.Username && session.category == "SUCCESS")
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

        public void LaunchGame()
        {
            if (!String.IsNullOrEmpty(LaunchArgs))
            {
                Process dcuo = new()
                {
                    StartInfo = new()
                    {
                        FileName = dcuoLaunchmefirstPath,
                        Arguments = LaunchArgs,
                        UseShellExecute = false,
                        CreateNoWindow = false
                    }
                };
                dcuo.Start();
            }
        }
    }
}