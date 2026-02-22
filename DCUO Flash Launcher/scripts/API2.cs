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
        /// Stores the DCUO server statuses.
        /// </summary>
        public static List<GameServerStatus> DCUOServers { get; private set; } = new();

        public static List<ServerStatusViewModel> ServerStatusList { get; set; } = new();

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
        public static async Task Login(string region)
        {
            Debug.WriteLine("[ API2:Login] Starte Login() mit region: " + region);
            if (UserAccount == null)
            {
                throw new Exception("you need to provide a user account befor you can login");
            }

            // Session-Cookie abrufen
            Debug.WriteLine("[ API:Login ] Connecting to: " + URL.Live);
            HttpResponseMessage response = await Client.GetAsync(URL.Live); // Warten auf die Antwort
            Uri uri = new Uri(URL.Live);
            Cookies.SetCookies(uri, $"lp-version={region.ToLower()}");

            IEnumerable<Cookie> responseCookies = Cookies.GetCookies(uri).Cast<Cookie>();
            foreach (Cookie cookie in responseCookies)
            {
                Debug.WriteLine("[ API:Login ] Set cookie: " + cookie.Name + ": " + cookie.Value);
            }

            // Login-Daten hinzufügen
            Debug.WriteLine("[ API:Login ] Adding Username " + UserAccount.Username + " and password to cookie");
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", UserAccount.Username),
                new KeyValuePair<string, string>("password", UserAccount.Password),
            });

            Debug.WriteLine("[ API:Login ] Posting data to: " + URL.Login);
            response = await Client.PostAsync(URL.Login, content); // Asynchroner Post-Aufruf
            string responseData = await response.Content.ReadAsStringAsync(); // Asynchron Daten lesen

            Debug.WriteLine("[ API:Login ] Post result: " + responseData);

            // Cookies erneut abrufen
            Debug.WriteLine("[ API:Login ] Getting current cookies...");
            uri = new Uri(URL.Login);
            responseCookies = Cookies.GetCookies(uri).Cast<Cookie>();

            int counter = 0;
            foreach (Cookie cookie in responseCookies)
            {
                Debug.WriteLine("[ API:Login ] Cookie Login: " + cookie.Name + ": " + cookie.Value);
                counter++;
            }

            // Login überprüfen
            bool isLoggedIn = CheckLogin(responseData); // Nutzung der Rückgabe

        }

        /// <summary>
        /// Calls /get_play_session and gets the launch arguments for the game
        /// Login needs to be run first
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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
        /// <param name="region"></param>
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

        /// <summary>
        /// Fetches and stores the server status for DCUO servers.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task FetchServerStatusAsync()
        {
            Debug.WriteLine("[ API2:FetchServerStatusAsync ] Fetching server status...");

            string serverStatusUrl = "https://census.daybreakgames.com/s:dgc/json/get/global/game_server_status" +
                                     "?c:show=name,game_code,region_name,last_reported_state,last_reported_time" +
                                     "&c:limit=1000";

            try
            {
                HttpResponseMessage response = await Client.GetAsync(serverStatusUrl);
                response.EnsureSuccessStatusCode(); // Throw if the HTTP response indicates failure
                string responseData = await response.Content.ReadAsStringAsync();

                // Parse JSON response
                var parsedData = JsonConvert.DeserializeObject<ServerStatusResponse>(responseData);
                if (parsedData != null && parsedData.GameServerStatusList != null)
                {
                    // Filter and store DCUO server statuses
                    var dcuoServers = parsedData.GameServerStatusList
                        .Where(server => server.GameCode == "dcuo")
                        .ToList();

                    Debug.WriteLine("[ API2:FetchServerStatusAsync ] Fetched DCUO server statuses:");
                    foreach (var server in dcuoServers)
                    {
                        Debug.WriteLine($"Server: {server.Name}, Region: {server.RegionName}, " +
                                        $"Status: {server.LastReportedState}, Last Checked: {server.LastReportedTime}");
                    }

                    // Store server status for later use
                    DCUOServers = dcuoServers;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ API2:FetchServerStatusAsync ] Error fetching server status: " + ex.Message);
                throw;
            }
        }

        public static async Task PopulateServerStatusAsync()
        {
            await FetchServerStatusAsync(); // Fetch server data
            ServerStatusList = DCUOServers.Select(server => new ServerStatusViewModel
            {
                ServerName = server.Name,
                ServerStatus = server.LastReportedState,
            }).ToList();
        }
    }
}