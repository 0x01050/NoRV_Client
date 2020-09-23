using OBSWebsocketDotNet;
using System;
using System.Diagnostics;

namespace NoRV
{
    class OBSManager
    {
        public static bool CheckOBSRunning()
        {
            // Using Web Socket
            var _obs = new OBSWebsocket();
            try
            {
                _obs.WSTimeout = TimeSpan.FromMilliseconds(defaultTimeout);
                _obs.Connect("ws://127.0.0.1:4444", "");
                if (_obs.IsConnected)
                {
                    _obs.Disconnect();
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.info("OBS Connection Failed", e.Message);
            }
            return false;
        }

        public static void StartOBSRecording(string witness = null)
        {
            OBSAction(Config.getInstance().getOBSHotkey("start"));
        }
        public static void StopOBSRecording(string witness = null)
        {
            OBSAction(Config.getInstance().getOBSHotkey("stop"));
        }
        public static void PauseOBSRecording(string witness = null)
        {
            OBSAction(Config.getInstance().getOBSHotkey("pause"));
        }
        public static void UnpauseOBSRecording(string witness = null)
        {
            OBSAction(Config.getInstance().getOBSHotkey("unpause"));
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
            catch(Exception e)
            {
                Logger.info("OBS Action Failed", e.Message);
            }

        }
    }
}
