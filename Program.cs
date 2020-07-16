using Accord;
using GoogleTranscribing;
using Grapevine.Server;
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
using System.Runtime.InteropServices;
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

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string port = Config.getInstance().getWebServerPort();
            var server = new RestServer(new ServerSettings()
            {
                Host = "*",
                Port = "9999",
                PublicFolder = new PublicFolder("WebServer")
            });
            while (true)
            {
                try
                {
                    server.Start();
                    break;
                }
                catch (Exception)
                {
                    ServerCheck(port);
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Thread thread = new Thread(new ThreadStart(reportThread));
            thread.Start();
            Thread updateInfo = new Thread(new ThreadStart(updateInfoThread));
            updateInfo.Start();
            Thread detectThread = new Thread(new ThreadStart(DetectWork));
            detectThread.Start();
            Thread mirrorThread = new Thread(new ThreadStart(MirrorWork));
            mirrorThread.Start();
            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Run(() => InfiniteStreaming.RecognizeAsync(cts), cts.Token);
            if (!OBSManager.CheckOBSRunning())
                Application.Run(new WaitScreen());
            OBSManager.StopOBSRecording();
            TrackForm track = new TrackForm();
            track.Show();
            Application.Run(new InfoScreen());
            track.Terminate();
            track.Close();
            cts.Cancel();
            mirrorThread.Abort();
            detectThread.Abort();
            updateInfo.Abort();
            thread.Abort();

            server.Stop();
        }

        private static void ServerCheck(string port)
        {
            try
            {
                string args = string.Format(@"http add urlacl url=http://*:{0}/ user=Everyone", port);

                ProcessStartInfo psi = new ProcessStartInfo("netsh", args);
                psi.Verb = "runas";
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = true;

                Process.Start(psi).WaitForExit();
            }
            catch(Exception) { }
        }


        // App Status
        private static string usage = "";
        private static string obs = "Awaiting Recording (Unknown)";
        private static string button = "";
        private static string depo = "Awaiting Start (Unknown)";
        private static List<string[]> witness = new List<string[]>();
 
        public static void changeOBS(string newOBS)
        {
            if(obs != newOBS)
            {
                obs = newOBS;
                reportStatus();
            }
        }
        public static void changeWitness(string id, string title, string time, int type)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string[] found = witness.Find(x => x[0] == id);
            if (found == null)
                witness.Add(new string[] { id, title, date, "", "" });
            else if (type == 1)
            {
                found[3] = time;
                found[4] = "Not concluded";
            }
            else if (type == 2)
            {
                found[4] = time;
            }
            else if (type == 3)
            {
                found[3] = "";
                found[4] = "";
            }
            else
                return;
            reportStatus();
        }

        private static void filterWitness()
        {
            witness.Sort((l, r) =>
            {
                if (l == null || l.Length != 5)
                    return 1;
                if (r == null || r.Length != 5)
                    return -1;
                if (l[3] == "")
                    return 1;
                if (r[3] == "")
                    return -1;
                return l[3].CompareTo(r[3]);
            });
            witness = witness.FindAll(x =>
            {
                if (x == null || x.Length != 5)
                    return false;
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                return x[2] == date;
            });
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
                filterWitness();
                if (witness.Count > 0)
                    joinedWitness += formatWitness(witness[0]);
                for(int i = 1; i < witness.Count; i ++)
                    joinedWitness += ", " + formatWitness(witness[i]);
                string jobs = witness.FindAll(x => (x != null && x.Length == 5 && x[3] == "")).Count.ToString();

                var httpClient = new HttpClient();
                string url = Config.getInstance().getServerUrl() + "/status/master";

                MultipartFormDataContent httpContent = new MultipartFormDataContent();
                httpContent.Add(new StringContent(L.v()), "id");
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
            if (onelog.Length > 1)
                curStr = onelog[1];
            if (onelog.Length > 3 && onelog[3] != "")
                curStr += " (" + onelog[3] + "/" + onelog[4] + ")";
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
                        gScreen.CopyFromScreen(s_rect.Location, System.Drawing.Point.Empty, s_rect.Size);
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
                    }
                }
                catch (Exception)
                {
                    videographer = Config.getInstance().getVideographer();
                    commission = Config.getInstance().getCommission();
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
                            "--fullscreen --window-borderless --window-title '" + Config.getInstance().getMirrorSourceWindow() + "'");
                        Thread.Sleep(2000);
                        if (proc.MainWindowHandle != null)
                        {
                            User32.SetWindowPos(proc.MainWindowHandle, User32.HWND_BOTTOM, 0, 0, 0, 0, User32.NOMOVE_NOSIZE_FLAG);
                        }
                    }
                    else
                    {
                        foreach(var proc in procs)
                        {
                            if(proc.MainWindowHandle != null)
                            {
                                User32.SetWindowPos(proc.MainWindowHandle, User32.HWND_BOTTOM, 0, 0, 0, 0, User32.NOMOVE_NOSIZE_FLAG);
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
                        Image capture = CaptureWindow(proc.MainWindowHandle);
                        var cloned = new Bitmap(capture);
                        if (prevBitmap != null)
                        {
                            var difference = CalculateDifference(prevBitmap, cloned);
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
                        Console.WriteLine(e.StackTrace);
                    }
                }
                if(!found)
                {
                    OBSManager.SwitchToWitness();
                }
                Thread.Sleep(100);
            }
        }
        static float CalculateDifference(Bitmap bitmap1, Bitmap bitmap2)
        {
            if (bitmap1.Size != bitmap2.Size)
            {
                return -1;
            }

            var rectangle = new Rectangle(0, 0, bitmap1.Width, bitmap1.Height);

            BitmapData bitmapData1 = bitmap1.LockBits(rectangle, ImageLockMode.ReadOnly, bitmap1.PixelFormat);
            BitmapData bitmapData2 = bitmap2.LockBits(rectangle, ImageLockMode.ReadOnly, bitmap2.PixelFormat);

            float diff = 0;
            var byteCount = rectangle.Width * rectangle.Height * 3;

            unsafe
            {
                byte* pointer1 = (byte*)bitmapData1.Scan0.ToPointer();
                byte* pointer2 = (byte*)bitmapData2.Scan0.ToPointer();

                for (int x = 0; x < byteCount; x++)
                {
                    diff += (float)Math.Abs(*pointer1 - *pointer2) / 255;
                    pointer1++;
                    pointer2++;
                }
            }

            bitmap1.UnlockBits(bitmapData1);
            bitmap2.UnlockBits(bitmapData2);

            return 100 * diff / byteCount;
        }
        private static Image CaptureWindow(IntPtr handle)
        {
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height - Config.getInstance().getMirrorIgnore());
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            GDI32.BitBlt(hdcDest, 0, 0, width, height - Config.getInstance().getMirrorIgnore(), hdcSrc, 0, Config.getInstance().getMirrorIgnore(), GDI32.SRCCOPY);
            GDI32.SelectObject(hdcDest, hOld);
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            Image img = Image.FromHbitmap(hBitmap);
            GDI32.DeleteObject(hBitmap);
            return img;
        }
        private class GDI32
        {

            public const int SRCCOPY = 0x00CC0020;
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

            [DllImport("user32.dll")]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
            public static readonly uint NOMOVE_NOSIZE_FLAG = 0x0001 | 0x0002;
            public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        }

    }
}
