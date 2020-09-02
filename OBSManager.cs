using OBSWebsocketDotNet;
using System;
using System.Diagnostics;

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
            OBSAction(Config.getInstance().getOBSHotkey("start"));

            Program.changeOBS("Recording (" + witness + ")");
        }
        public static void StopOBSRecording(string witness = null)
        {
            OBSAction(Config.getInstance().getOBSHotkey("stop"));

            if (witness != null)
                Program.changeOBS("Recording Ended (" + witness + ")");
            else
                Program.changeOBS("Awaiting Recording (Unknown)");
        }
        public static void PauseOBSRecording(string witness = null)
        {
            OBSAction(Config.getInstance().getOBSHotkey("pause"));

            Program.changeOBS("Recording Paused (" + witness + ")");
        }
        public static void UnpauseOBSRecording(string witness = null)
        {
            OBSAction(Config.getInstance().getOBSHotkey("unpause"));

            Program.changeOBS("Recording (" + witness + ")");
        }
        public static void SwitchToWitness()
        {
            OBSAction(Config.getInstance().getOBSHotkey("witness"));
        }
        public static void SwitchToExhibits()
        {
            OBSAction(Config.getInstance().getOBSHotkey("exhibits"));
        }

        private static int defaultTimeout = 300;
        private static void OBSAction(string action)
        {
            //if (!CheckOBSRunning())
            //    return;

            // Using Web Socket
            var _obs = new OBSWebsocket();
            try
            {
                _obs.WSTimeout = TimeSpan.FromMilliseconds(defaultTimeout);
                _obs.Connect("ws://127.0.0.1:4444", "");
                if (_obs.IsConnected)
                {
                    if (action.Contains("Recording"))
                    {
                        _obs.SendRequest(action);
                    }
                    else
                        _obs.SetCurrentScene(action);
                    _obs.Disconnect();
                }
            }
            catch
            {
                Console.WriteLine("*************************** OBS Exception ***************************");
            }

        }
    }
}
