using Accord.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NoRV
{
    class Config
    {
        private static string xmlFile = @"Config.xml";
        private static Config _instance = null;
        public static Config getInstance()
        {
            if (_instance == null)
            {
                _instance = new Config();
            }
            return _instance;
        }


        Config()
        {
            LoadGlobalConfig();
            LoadOBSConfig();
        }


        // Global Config
        private int _defaultVolume = 65;
        private string _announceTime = "";
        private string _logPath = "";
        private string _videoPath = "";
        private int _alertInterval = 100;
        private int _alertVolume = 40;
        private string _totalTimeTemplate = "";
        private string _endTimeTemplate = "";
        private string _serverUrl = "";
        private string _videographer = "";
        private string _commission = "";
        private string _autoStart = "true";
        private string _googleVoiceName = "";
        private double _googleVoiceSpeed = 0.9;
        private double _googleVoicePitch = 0;
        private void LoadGlobalConfig()
        {
            var xml = XDocument.Load(xmlFile);
            var query = from c in xml.Root.Descendants("Item")
                        select c;
            foreach (var item in query)
            {
                switch ((string)item.Attribute("Key"))
                {
                    case "DefaultVolume":
                        _defaultVolume = (int)item.Attribute("Value");
                        break;
                    case "AnnounceTime":
                        _announceTime = (string)item.Attribute("Value");
                        break;
                    case "LogPath":
                        _logPath = (string)item.Attribute("Value");
                        break;
                    case "VideoPath":
                        _videoPath = (string)item.Attribute("Value");
                        break;
                    case "BreakAlertInterval":
                        _alertInterval = (int)item.Attribute("Value");
                        break;
                    case "BreakAlertVolume":
                        _alertVolume = (int)item.Attribute("Value");
                        break;
                    case "TotalTimeAudioTemplate":
                        _totalTimeTemplate = (string)item.Attribute("Value");
                        break;
                    case "EndTimeAudioTemplate":
                        _endTimeTemplate = (string)item.Attribute("Value");
                        break;
                    case "ServerUrl":
                        _serverUrl = (string)item.Attribute("Value");
                        break;
                    case "Videographer":
                        _videographer = (string)item.Attribute("Value");
                        break;
                    case "Commission":
                        _commission = (string)item.Attribute("Value");
                        break;
                    case "AutoStart":
                        _autoStart = (string)item.Attribute("Value");
                        break;
                    case "GoogleVoice":
                        _googleVoiceName = (string)item.Attribute("Name");
                        _googleVoiceSpeed = (double)item.Attribute("Speed");
                        _googleVoicePitch = (double)item.Attribute("Pitch");
                        break;
                }
            }
        }
        public int getDefaultVolume()
        {
            return _defaultVolume;
        }
        public string getAnnounceTime()
        {
            return _announceTime;
        }
        public string getLogPath()
        {
            return _logPath;
        }
        public string getVideoPath()
        {
            return _videoPath;
        }
        public int getAlertInterval()
        {
            return _alertInterval;
        }
        public int getAlertVolume()
        {
            return _alertVolume;
        }
        public string getTotalTimeTemplate()
        {
            return _totalTimeTemplate;
        }
        public string getEndTimeTemplate()
        {
            return _endTimeTemplate;
        }
        public string getServerUrl()
        {
            return _serverUrl;
        }
        public string getVideographer()
        {
            return _videographer;
        }
        public string getCommission()
        {
            return _commission;
        }
        public bool getAutoStart()
        {
            try
            {
                string autostart = _autoStart.ToLower();
                if (autostart == "true")
                    return true;
            }
            catch (Exception) { }
            return false;
        }
        public string getGoogleVoiceName()
        {
            return _googleVoiceName;
        }
        public double getGoogleVoiceSpeed()
        {
            return _googleVoiceSpeed;
        }
        public double getGoogleVoicePitch()
        {
            return _googleVoicePitch;
        }


        // OBS Config
        private string _startHotkey = "R";
        private string _stopHotkey = "S";
        private string _pauseHotkey = "P";
        private string _unpauseHotkey = "U";
        private string _witnessHotkey = "1";
        private string _exhibitsHotkey = "2";

        // Mirror
        private string _mirrorSourceProcess = "Mirror";
        private string _mirrorSourceWindow = "*";
        private Size _mirrorResolution = new Size(1280, 720);
        private int _mirrorIgnore = 10;
        private double _detectThreashold = 0;
        private int _switchTime = 5000;

        private void LoadOBSConfig()
        {
            var xml = XDocument.Load(xmlFile);
            var query = from c in xml.Root.Descendants("Config")
                        select c;
            foreach (var item in query)
            {
                switch ((string)item.Attribute("Key"))
                {
                    case "StartHotkey":
                        _startHotkey = (string)item.Attribute("Value");
                        break;
                    case "StopHotkey":
                        _stopHotkey = (string)item.Attribute("Value");
                        break;
                    case "PauseHotkey":
                        _pauseHotkey = (string)item.Attribute("Value");
                        break;
                    case "UnpauseHotkey":
                        _unpauseHotkey = (string)item.Attribute("Value");
                        break;
                    case "WitnessHotkey":
                        _witnessHotkey = (string)item.Attribute("Value");
                        break;
                    case "ExhibitsHotkey":
                        _exhibitsHotkey = (string)item.Attribute("Value");
                        break;

                    case "MirrorSource":
                        _mirrorSourceProcess = (string)item.Attribute("Process");
                        _mirrorSourceWindow = (string)item.Attribute("Window");
                        break;
                    case "MirrorResolution":
                        _mirrorResolution = new Size((int)item.Attribute("Width"), (int)item.Attribute("Height"));
                        _mirrorIgnore = (int)item.Attribute("Ignore");
                        break;
                    case "DetectThreashold":
                        _detectThreashold = (double)item.Attribute("Value");
                        break;
                    case "SwitchTime":
                        _switchTime = (int)item.Attribute("Value");
                        break;

                }
            }
        }
        public string getOBSHotkey(string action)
        {
            switch (action)
            {
                case "start":
                    return _startHotkey;
                case "stop":
                    return _stopHotkey;
                case "pause":
                    return _pauseHotkey;
                case "unpause":
                    return _unpauseHotkey;
                case "witness":
                    return _witnessHotkey;
                case "exhibits":
                    return _exhibitsHotkey;
            }
            return "";
        }

        public string getMirrorSourceProcess()
        {
            return _mirrorSourceProcess;
        }
        public string getMirrorSourceWindow()
        {
            return _mirrorSourceWindow;
        }
        public Size getMirrorResolution()
        {
            return _mirrorResolution;
        }
        public int getMirrorIgnore()
        {
            return _mirrorIgnore;
        }
        public double getDetectThreshold()
        {
            return _detectThreashold;
        }
        public int getSwitchTime()
        {
            return _switchTime;
        }

        public string getTemplate(string template)
        {
            var xml = XDocument.Load(xmlFile);
            var query = from c in xml.Root.Descendants("Template")
                        where (string)c.Attribute("Type") == template
                        select c.Value.ToString()
                                .Replace("      ", "")
                                .Replace("\n", "\r\n")
                                .Trim();
            foreach (var item in query)
            {
                return item;
            }
            return "";
        }

        public void getTimezone(string tz, ref string tzId, ref bool daylight, ref int offset)
        {
            try
            {
                var xml = XDocument.Load(xmlFile);
                var query = from c in xml.Root.Descendants("Timezone")
                            where (string)c.Attribute("Name") == tz
                            select c;
                foreach (var item in query)
                {
                    tzId = (string)item.Attribute("Id");
                    daylight = (bool)item.Attribute("Daylight");
                    offset = (int)item.Attribute("Offset");
                    return;
                }
            }
            catch { }
            tzId = ""; daylight = false; offset = 0;
        }
    }
}
