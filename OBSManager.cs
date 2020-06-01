using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoRV
{
    class OBSManager
    {
        private static IntPtr noWindow = (IntPtr)0;
        private static IntPtr getOBSMainWindow()
        {
            Process[] obs64 = Process.GetProcessesByName(Config.getInstance().getOBSProcessName());
            foreach (Process obs in obs64)
            {
                if (obs.MainWindowHandle != (IntPtr)0)
                {
                    return obs.MainWindowHandle;
                }
            }
            return noWindow;
        }
        public static bool CheckOBSRunning()
        {
            return (getOBSMainWindow() != noWindow);
        }

        public static void StartOBSRecording()
        {
            if(!CheckOBSRunning())
            {
                return;
            }
            IntPtr mainWindow = getOBSMainWindow();
            SetForegroundWindow(mainWindow);
            Thread.Sleep(30);
            SendKeys.SendWait(Config.getInstance().getOBSHotkey("start"));
        }

        public static void StopOBSRecording()
        {
            if (!CheckOBSRunning())
            {
                return;
            }
            IntPtr mainWindow = getOBSMainWindow();
            SetForegroundWindow(mainWindow);
            Thread.Sleep(30);
            SendKeys.SendWait(Config.getInstance().getOBSHotkey("stop"));
        }

        public static void PauseOBSRecording()
        {
            if (!CheckOBSRunning())
            {
                return;
            }
            IntPtr mainWindow = getOBSMainWindow();
            SetForegroundWindow(mainWindow);
            Thread.Sleep(30);
            SendKeys.SendWait(Config.getInstance().getOBSHotkey("pause"));
        }

        public static void UnpauseOBSRecording()
        {
            if (!CheckOBSRunning())
            {
                return;
            }
            IntPtr mainWindow = getOBSMainWindow();
            SetForegroundWindow(mainWindow);
            Thread.Sleep(30);
            SendKeys.SendWait(Config.getInstance().getOBSHotkey("unpause"));
        }

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
