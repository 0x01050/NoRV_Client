using System;
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
            else
                Program.changeOBS("Awaiting Recording (Unknown)");
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
        public static void SwitchToWitness()
        {
            SendHotkey(Config.getInstance().getOBSHotkey("witness"));
        }
        public static void SwitchToExhibits()
        {
            SendHotkey(Config.getInstance().getOBSHotkey("exhibits"));
        }

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        private static void SendHotkey(string hotkey)
        {
            hotkey = hotkey.ToUpper();
            if (hotkey.Length > 0 && Char.IsLetterOrDigit(hotkey[0]))
            {
                int vk;
                if (Char.IsDigit(hotkey[0]))
                    vk = 0x30 + hotkey[0] - '0';
                else if (Char.IsLower(hotkey[0]))
                    vk = 0x41 + hotkey[0] - 'a';
                else
                    vk = 0x41 + hotkey[0] - 'A';
                keybd_event(0x11, 0, 0, 0);
                keybd_event((byte)vk, 0, 0, 0);
                Thread.Sleep(75);
                keybd_event(0x11, 0, 2, 0);
                keybd_event((byte)vk, 0, 2, 0);
            }
        }
    }
}
