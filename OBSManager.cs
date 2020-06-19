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
        public static bool CheckOBSRunning()
        {
            Process[] obs64 = Process.GetProcessesByName(Config.getInstance().getOBSProcessName());
            return obs64.Length > 0;
        }

        public static void StartOBSRecording()
        {
            SendHotkey(Config.getInstance().getOBSHotkey("start"));
        }
        public static void StopOBSRecording()
        {
            SendHotkey(Config.getInstance().getOBSHotkey("stop"));
        }
        public static void PauseOBSRecording()
        {
            SendHotkey(Config.getInstance().getOBSHotkey("pause"));
        }
        public static void UnpauseOBSRecording()
        {
            SendHotkey(Config.getInstance().getOBSHotkey("unpause"));
        }

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        private static void SendHotkey(string hotkey)
        {
            hotkey = hotkey.ToUpper();
            if (hotkey.Length > 0 && hotkey[0] >= 'A' && hotkey[0] <= 'Z')
            {
                int vk = hotkey[0] - 'A';
                keybd_event(0x11, 0, 0, 0);
                keybd_event((byte)(0x41 + vk), 0, 0, 0);
                Thread.Sleep(75);
                keybd_event(0x11, 0, 2, 0);
                keybd_event((byte)(0x41 + vk), 0, 2, 0);
            }
        }
    }
}
