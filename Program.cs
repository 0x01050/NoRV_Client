using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;

namespace NoRV
{
    static class Program
    {
        public static bool DEBUG = false;

        public static string videographer = "";
        public static string commission = "";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            WebServer webServer = new WebServer(WebServer.SendResponse, "http://127.0.0.1:4001/now/");
            webServer.Run();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            RegisterScreen prompt = new RegisterScreen();
            Application.Run(prompt);
            if (prompt.DialogResult != DialogResult.OK)
                return;

            Thread thread = new Thread(new ThreadStart(reportThread));
            thread.Start();
            Thread updateInfo = new Thread(new ThreadStart(updateInfoThread));
            updateInfo.Start();
            if (!OBSManager.CheckOBSRunning())
                Application.Run(new WaitScreen());
            OBSManager.StopOBSRecording();
            Application.Run(new InfoScreen());
            updateInfo.Abort();
            thread.Abort();
        }


        // App Status
        private static string usage = "";
        private static string obs = "Awaiting Recording (Unknown)";
        private static string button = "";
        private static string depo = "Awaiting Start (Unknown)";
        private static string jobs = "0";
        private static List<string[]> witness = new List<string[]>();
 
        public static void changeOBS(string newOBS)
        {
            if(obs != newOBS)
            {
                obs = newOBS;
                reportStatus();
            }
        }
        public static void changeDepo(string newDepo)
        {
            if(depo != newDepo)
            {
                depo = newDepo;
                reportStatus();
            }
        }
        public static void changeJobs(string newJob)
        {
            if(jobs != newJob)
            {
                jobs = newJob;
                reportStatus();
            }
        }
        public static void changeWitness(string title, string time, bool start)
        {
            if(start)
                witness.Add(new string[] { title, time, "Not concluded" });
            else
            {
                int idx = witness.Count - 1;
                if (idx >= 0)
                    witness[idx][2] = time;
            }
            reportStatus();
        }

        private static PerformanceCounter theCPUCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
        private static PerformanceCounter theMemCounter = new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName);
        public static void reportStatus()
        {
            try
            {
                string CPUUsage = Math.Round(theCPUCounter.NextValue(), 1) + "%";
                string RAMUsage = Math.Round(theMemCounter.NextValue() / 1048576, 1) + "MB";
                usage = CPUUsage + " / " + RAMUsage;
                button = ButtonManager.getInstance().getButtonStatus() == ButtonManager.INITIATED ? "Operational" : "Not Operational";
                string screenshot = CaptureScreen(Screen.PrimaryScreen);
                string joinedWitness = "";
                if (witness.Count > 0)
                    joinedWitness += formatWitness(witness[0]);
                for(int i = 1; i < witness.Count; i ++)
                    joinedWitness += ", " + formatWitness(witness[i]);

                var httpClient = new HttpClient();
                string url = Config.getInstance().getServerUrl() + "/status/master";

                MultipartFormDataContent httpContent = new MultipartFormDataContent();
                httpContent.Add(new StringContent(L.getID()), "id");
                httpContent.Add(new StringContent(usage), "usage");
                httpContent.Add(new StringContent(obs), "obs");
                httpContent.Add(new StringContent(button), "button");
                httpContent.Add(new StringContent(depo), "depo");
                httpContent.Add(new StringContent(jobs), "jobs");
                httpContent.Add(new StringContent(joinedWitness), "witness");
                httpContent.Add(new StringContent(screenshot), "screenshot");
                httpClient.PostAsync(url, httpContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        private static string formatWitness(string[] onelog)
        {
            string curStr = "";
            if (onelog.Length > 0)
                curStr = onelog[0];
            if (onelog.Length > 2)
                curStr += " (" + onelog[1] + "/" + onelog[2] + ")";
            return curStr;
        }
        private static string CaptureScreen(Screen window)
        {
            try
            {
                Rectangle s_rect = window.Bounds;
                using (Bitmap bmp = new Bitmap(s_rect.Width, s_rect.Height))
                {
                    using (Graphics gScreen = Graphics.FromImage(bmp))
                        gScreen.CopyFromScreen(s_rect.Location, Point.Empty, s_rect.Size);
                    using (Bitmap newBmp = new Bitmap(bmp, new Size(428, 240)))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            newBmp.Save(ms, ImageFormat.Png);
                            byte[] byteImage = ms.ToArray();
                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception) { }
            return "";
        }
        private static void reportThread()
        {
            while(true)
            {
                reportStatus();
                Thread.Sleep(30 * 1000);
            }
        }

        private static async void updateInfoThread()
        {
            while(true)
            {
                try
                {
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(Config.getInstance().getServerUrl() + "/info");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var respObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                        foreach (var item in respObj)
                        {
                            switch (item.Key)
                            {
                                case "Videographer":
                                    videographer = item.Value;
                                    break;
                                case "Commission":
                                    commission = item.Value;
                                    break;
                            }
                        }
                    }
                }
                catch (Exception) { }
                Thread.Sleep(1 * 1000);
            }
        }
    }
}
