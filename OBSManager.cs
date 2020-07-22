﻿using OBSWebsocketDotNet;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static NoRV.Program;

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

        private static int sceneMode = -1;
        public static void SwitchToWitness()
        {
            if (sceneMode != 0)
            {
                sceneMode = 0;
                SendHotkey(Config.getInstance().getOBSHotkey("witness"));
            }
        }
        public static void SwitchToExhibits()
        {
            if (sceneMode != 1)
            {
                sceneMode = 1;
                SendHotkey(Config.getInstance().getOBSHotkey("exhibits"));
            }
        }

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        private static bool _hotkeyRunning = false;
        private static async Task<int> SendHotkey(string hotkey)
        {
            // Using Web Socket
            var _obs = new OBSWebsocket();
            try
            {
                _obs.Connect("ws://127.0.0.1:4444", "");
                switch (hotkey)
                {
                    case "Start":
                        _obs.StartRecording();
                        break;
                    case "Stop":
                        _obs.StopRecording();
                        break;
                    default:
                        _obs.SetCurrentScene(hotkey);
                        break;
                }
                _obs.Disconnect();
            }
            catch(Exception) { }

            // Using Hotkey
            //using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            //{
            //    int timeout = 1000;
            //    var task = TriggerHotkey(hotkey, timeoutCancellationTokenSource);

            //    var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
            //    if (completedTask == task)
            //    {
            //        timeoutCancellationTokenSource.Cancel();
            //        return await task;
            //    }
            //}

            return 0;
        }
        private static async Task<int> TriggerHotkey(string hotkey, CancellationTokenSource cts)
        {
            while (_hotkeyRunning)
            {
                if (cts.IsCancellationRequested)
                    return 0;
            }
            Console.WriteLine($"HotKey Pressed - {hotkey}");
            _hotkeyRunning = true;
            Process[] obs64 = Process.GetProcessesByName(Config.getInstance().getOBSProcessName());
            foreach(var obs in obs64)
            {
                if (obs.MainWindowHandle != null)
                {
                    User32.SetWindowPos(obs.MainWindowHandle, User32.HWND_TOP, 0, 0, 0, 0, User32.NOMOVE_NOSIZE_SHOW_FLAG);
                }
            }
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
            _hotkeyRunning = false;
            return 0;
        }
    }
}
