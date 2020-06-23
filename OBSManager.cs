using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace NoRV
{
    class OBSManager
    {
        public static bool CheckOBSRunning()
        {
            Process[] obs64 = Process.GetProcessesByName(Config.getInstance().getOBSProcessName());
            return obs64.Length > 0;
        }

        public static void StartOBSRecording(string witness = null)
        {
            SendHotkey(Config.getInstance().getOBSHotkey("start"));

            Program.changeOBS("Recording (" + witness + ")");
        }
        public static void StopOBSRecording(string witness = null)
        {
            SendHotkey(Config.getInstance().getOBSHotkey("stop"));

            if (witness != null)
                Program.changeOBS("Recording Ended (" + witness + ")");
        }
        public static void PauseOBSRecording(string witness = null)
        {
            SendHotkey(Config.getInstance().getOBSHotkey("pause"));

            Program.changeOBS("Recording Paused (" + witness + ")");
        }
        public static void UnpauseOBSRecording(string witness = null)
        {
            SendHotkey(Config.getInstance().getOBSHotkey("unpause"));

            Program.changeOBS("Recording (" + witness + ")");
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
