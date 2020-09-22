using GoogleTranscribing;
using Grapevine.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoRV
{
    static class Program
    {
        public static bool DEBUG = false;

        public static string videographer = "";
        public static string commission = "";

        [STAThread]
        static void Main()
        {
            Logger.info("Starting NoRV");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool autoStart = Config.getInstance().getAutoStart();
            Logger.info("Checking AutoStart is Enabled", autoStart.ToString());
            if(!autoStart)
            {
                Logger.info("Exiting from NoRV", "AutoStart is Disabled");
                Application.Exit();
                return;
            }

            Logger.info("Startign HTTP Server");
            var http = new RestServer(new ServerSettings()
            {
                Host = "*",
                Port = "80",
                PublicFolder = new PublicFolder("WebServer")
            });
            try
            {
                http.Start();
                Logger.info("HTTP Server Started");
            }
            catch (Exception e)
            {
                Logger.info("HTTP Server Starting Failed", e.Message);
            }

            //Thread thread = new Thread(new ThreadStart(reportThread));
            //thread.Start();
            //Logger.info("Report Thread Started");

            Thread updateInfo = new Thread(new ThreadStart(updateInfoThread));
            updateInfo.Start();
            Logger.info("Update Thread Started");

            Thread detectThread = new Thread(new ThreadStart(DetectWork));
            detectThread.Start();
            Logger.info("Face Detect Thread Started");
            
            Utils.Restart3rdParty("Track");
            Logger.info("Track Form Started");

            Thread mirrorThread = new Thread(new ThreadStart(MirrorWork));
            mirrorThread.Start();
            Logger.info("Mirror Thread Started");

            TranscribeManager.Stop();
            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Run(new Action(() => InfiniteStreaming.RecognizeAsync(cts, TranscribeManager.NewTranscript, TranscribeManager.NewCandidate)), cts.Token);
            Logger.info("Google Transcribing Started");

            if (!OBSManager.CheckOBSRunning())
            {
                Logger.info("Wait till OBS starts running");
                Application.Run(new WaitScreen());
                Logger.info("OBS Started");
            }
            Logger.info("Stop recording on OBS");
            OBSManager.StopOBSRecording();
            Logger.info("NoRV Fully Started");
//  ####################################################################  //
/**/        Application.Run(NoRVAppContext.getInstance());              /**/
//  ####################################################################  //
            Logger.info("Finishing NoRV");
            Utils.Stop3rdParty("Track");
            Logger.info("Track Form Ended");

            mirrorThread.Abort();
            Logger.info("Mirror Thread Ended");

            detectThread.Abort();
            Logger.info("Face Detect Thread Ended");

            updateInfo.Abort();
            Logger.info("Update Thread Finished");

            //thread.Abort();
            //Logger.info("Report Thread Finished");

            try
            {
                if (http.IsListening)
                {
                    http.Stop();
                    Logger.info("HTTP Server Stopped");
                }
                else
                    Logger.info("HTTP Server Not Stopped", "Not Started");
            }
            catch (Exception e)
            {
                Logger.info("HTTP Server Not Stopped", e.Message);
            }

            Utils.Stop3rdParty("adb");
            Logger.info("Fully Terminate Mirror");

            cts.Cancel();
            Logger.info("Google Transcribing Finished");

            Application.Exit();
            Logger.info("NoRV Existed");
        }

        private static async void updateInfoThread()
        {
            while (true)
            {
                try
                {
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(Config.getInstance().getServerUrl() + "/info");
                    videographer = Config.getInstance().getVideographer();
                    commission = Config.getInstance().getCommission();
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
                        Logger.info("Videographer and Commission Updated", "Videographer: " + videographer + ", Commission: " + commission);
                    }
                }
                catch (Exception e)
                {
                    Logger.info("Videographer and Commission Updating Failed", e.Message);
                }
                if (String.IsNullOrEmpty(videographer))
                {
                    videographer = Config.getInstance().getVideographer();
                    Logger.info("Videographer Initiated", videographer);
                }
                if(String.IsNullOrEmpty(commission))
                {
                    commission = Config.getInstance().getCommission();
                    Logger.info("Commission Initiated", commission);
                }
                Thread.Sleep(1 * 1000);
            }
        }

        private static void MirrorWork()
        {
            while(true)
            {
                try
                {
                    Process[] procs = Process.GetProcessesByName(Config.getInstance().getMirrorSourceProcess());
                    if (procs.Length == 0)
                    {
                        Process proc = Process.Start("Mirror\\" + Config.getInstance().getMirrorSourceProcess() + ".exe", 
                            "--fullscreen --max-size 1024 --window-borderless --window-title '" + Config.getInstance().getMirrorSourceWindow() + "'");
                    }
                    else
                    {
                        foreach(var proc in procs)
                        {
                            if(proc.MainWindowHandle != null)
                            {
                                WinSDK.User32.SetWindowPos(proc.MainWindowHandle, WinSDK.User32.HWND_BOTTOM, 0, 0, 0, 0, WinSDK.User32.NOMOVE_NOSIZE_SHOW_FLAG);
                            }
                        }
                    }
                }
                catch (Exception) { }
                Thread.Sleep(5000);
            }
        }

        private static void DetectWork()
        {
            Bitmap prevBitmap = null;
            DateTime lastChanged = DateTime.Now.AddSeconds(-10);

            while (true)
            {
                bool found = false;
                Process[] processlist = Process.GetProcessesByName(Config.getInstance().getMirrorSourceProcess());
                foreach (Process proc in processlist)
                {
                    try
                    {
                        Image capture = Utils.CaptureWindow(proc.MainWindowHandle);
                        var cloned = new Bitmap(capture);
                        if (prevBitmap != null)
                        {
                            var difference = Utils.CalculateDifference(prevBitmap, cloned);
                            if (difference > Config.getInstance().getDetectThreshold())
                            {
                                OBSManager.SwitchToExhibits();
                                lastChanged = DateTime.Now;
                            }
                            else
                            {
                                TimeSpan span = DateTime.Now - lastChanged;
                                if (span.TotalMilliseconds > Config.getInstance().getSwitchTime())
                                {
                                    OBSManager.SwitchToWitness();
                                }
                            }
                        }
                        prevBitmap = cloned;
                        found = true;
                        break;
                    }
                    catch(Exception e)
                    {
                        Logger.info("Mirror Detect Failed", e.Message);
                    }
                }
                if(!found)
                {
                    OBSManager.SwitchToWitness();
                }
                Thread.Sleep(100);
            }
        }

        //private static void reportThread()
        //{
        //    while(true)
        //    {
        //        StatusManage.getInstance().reportBase();
        //        Thread.Sleep(30 * 1000);
        //    }
        //}
    }
}
