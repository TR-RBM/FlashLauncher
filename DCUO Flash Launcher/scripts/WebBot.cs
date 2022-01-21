using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Newtonsoft.Json;
using System.IO;

namespace FlashLauncher
{
    public class WebBot
    {
        FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
        string status = new("");
        string dcuoLaunchmefirstPath = new(@"C:\Users\Public\Daybreak Game Company\Installed Games\DC Universe Online\UNREAL3\BINARIES\WIN32\LAUNCHMEFIRST.EXE");

        public WebBot()
        {
            if (!File.Exists(@"C:\Users\Public\Daybreak Game Company\Installed Games\DC Universe Online\UNREAL3\BINARIES\WIN32\LAUNCHMEFIRST.EXE"))
            {
                Environment.Exit(0);
            }
            service.HideCommandPromptWindow = true;
        }

        public bool Login(Account account)
        {
            // add headless argument to firefox, so there will be no debug window
            FirefoxOptions options = new FirefoxOptions();
            options.AddArgument("--headless");

            // initiate the firefox driver
            FirefoxDriver firefoxDriver = new(service, options);
            IWebDriver driver = firefoxDriver;

            // open the login page of DCUO
            driver.Navigate().GoToUrl("https://lpj.daybreakgames.com/dcuo/live/");
            IWebElement loginUsername = driver.FindElement(By.XPath("//*[@id=\"loginUsername\"]"));
            loginUsername.SendKeys(account.Username);
            IWebElement loginPassword = driver.FindElement(By.XPath("//*[@id=\"loginPassword\"]"));
            loginPassword.SendKeys(account.Password);
            IWebElement loginSubmit = driver.FindElement(By.XPath("//*[@id=\"loginSubmit\"]"));
            loginSubmit.Click();

            // wait for the page to send a response
            Thread.Sleep(500);

            // get status from play session page
            driver.Navigate().GoToUrl("https://lpj.daybreakgames.com/dcuo/live/get_play_session");
            Thread.Sleep(500);
            IWebElement rawDataTab = driver.FindElement(By.XPath("//*[@id=\"rawdata-tab\"]"));
            rawDataTab.Click();
            IWebElement rawData = driver.FindElement(By.ClassName("data"));
            status = rawData.Text;
            
            // exit the driver
            driver.Close();
            driver.Quit();

            // check if the login was a SUCCESS
            if (IsLoggedIn(account))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsLoggedIn(Account account)
        {
            PlaySession? session = new PlaySession();
            if (String.IsNullOrEmpty(status))
            {
                return false;
            }
            try
            {
                session = JsonConvert.DeserializeObject<PlaySession>(status);
                Debug.WriteLine(session.username + " " + session.category);
                if (session.username == account.Username && session.category == "SUCCESS")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void LaunchGame(Account account)
        {
            PlaySession? session = new PlaySession();
            session = JsonConvert.DeserializeObject<PlaySession>(status);
            if (!String.IsNullOrEmpty(session.launchArgs))
            {
                Process dcuo = new()
                {
                    StartInfo = new()
                    {
                        FileName = dcuoLaunchmefirstPath,
                        Arguments = session.launchArgs,
                        UseShellExecute = false,
                        CreateNoWindow = false
                    }
                };
                dcuo.Start();
            }
        }
    }
}