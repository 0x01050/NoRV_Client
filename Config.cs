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
            LoadMappingConfig();
            LoadGlobalConfig();
            LoadOBSConfig();
        }

        // Mapping Config
        private string _machineIDKey = "";
        private string _witnessKey = "";
        private List<string> _keyList = new List<string>();
        private List<string> _nameList = new List<string>();
        private void LoadMappingConfig()
        {
            var xml = XDocument.Load(xmlFile);
            var query = from c in xml.Root.Descendants("Keyword")
                        select c;
            foreach (var item in query)
            {
                if ((string)item.Attribute("Key") == "MachineID")
                {
                    _machineIDKey = (string)item.Attribute("Name");
                }
                else
                {
                    if ((string)item.Attribute("Key") == "Witness")
                    {
                        _witnessKey = (string)item.Attribute("Name");
                    }
                    _keyList.Add((string)item.Attribute("Key"));
                    _nameList.Add((string)item.Attribute("Name"));
                }
            }
        }
        public string getMachineIDKey()
        {
            return _machineIDKey;
        }
        public string getWitnessKey()
        {
            return _witnessKey;
        }
        public string[] getKeyList()
        {
            return _keyList.ToArray();
        }
        public string getPairName(int idx)
        {
            if (idx < 0 || idx >= _nameList.Count)
                return "";
            return _nameList[idx];
        }


        // Global Config
        private int _buttonClickThreshold = 3;
        private int _defaultVolume = 50;
        private string _announceTime = "";
        private int _flashPeriod = 100;
        private int _pulsatePeriod = 1000;
        private string _logPath = "";
        private int _alertInterval = 300;
        private int _alertVolume = 25;
        private string _selectTemplate = "";
        private string _startTemplate = "";
        private string _totalTimeTemplate = "";
        private string _endTimeTemplate = "";
        private string _aucityAPIUrl = "";
        private int _aucityFetchInterval = 60;
        private string _aucityAPIUser = "";
        private string _aucityAPIPass = "";
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
                    case "FlashPeriod":
                        _flashPeriod = (int)item.Attribute("Value");
                        break;
                    case "PulsatePeriod":
                        _pulsatePeriod = (int)item.Attribute("Value");
                        break;
                    case "LogPath":
                        _logPath = (string)item.Attribute("Value");
                        break;
                    case "BreakAlertInterval":
                        _alertInterval = (int)item.Attribute("Value");
                        break;
                    case "BreakAlertVolume":
                        _alertVolume = (int)item.Attribute("Value");
                        break;
                    case "SelectWitnessAudioTemplate":
                        _selectTemplate = (string)item.Attribute("Value");
                        break;
                    case "StartAudioTemplate":
                        _startTemplate = (string)item.Attribute("Value");
                        break;
                    case "TotalTimeAudioTemplate":
                        _totalTimeTemplate = (string)item.Attribute("Value");
                        break;
                    case "EndTimeAudioTemplate":
                        _endTimeTemplate = (string)item.Attribute("Value");
                        break;
                    case "AucityAPIUrl":
                        _aucityAPIUrl = (string)item.Attribute("Value");
                        break;
                    case "AucityFetchInterval":
                        _aucityFetchInterval = (int)item.Attribute("Value");
                        break;
                    case "AucityAPIUser":
                        _aucityAPIUser = (string)item.Attribute("Value");
                        break;
                    case "AucityAPIPass":
                        _aucityAPIPass = (string)item.Attribute("Value");
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
        public int getButtonClickThreshold()
        {
            return _buttonClickThreshold;
        }
        public int getDefaultVolume()
        {
            return _defaultVolume;
        }
        public string getAnnounceTime()
        {
            return _announceTime;
        }
        public int getFlashPeriod()
        {
            return _flashPeriod;
        }
        public int getPulsatePeriod()
        {
            return _pulsatePeriod;
        }
        public string getLogPath()
        {
            return _logPath;
        }
        public int getAlertInterval()
        {
            return _alertInterval;
        }
        public int getAlertVolume()
        {
            return _alertVolume;
        }
        public string getSelectTemplate()
        {
            return _selectTemplate;
        }
        public string getStartTemplate()
        {
            return _startTemplate;
        }
        public string getTotalTimeTemplate()
        {
            return _totalTimeTemplate;
        }
        public string getEndTimeTemplate()
        {
            return _endTimeTemplate;
        }
        public string getAucityAPIUrl()
        {
            return _aucityAPIUrl;
        }
        public int getAucityFetchInterval()
        {
            return _aucityFetchInterval;
        }
        public string getAucityAPIUser()
        {
            return _aucityAPIUser;
        }
        public string getAucityAPIPass()
        {
            return _aucityAPIPass;
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
            catch(Exception) { }
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
        private string _obsProcess = "obs64";
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
        // Face Detect
        private string _cameraName = "";
        private int _detectSpeed = 800;
        private int _ignoreTop = 25;
        private int _ignoreBottom = 25;
        private Size _detectMainArea = new Size(400, 400);
        private int _detectOutsideSec = 60;
        private Size _inputResolution = new Size(1920, 1080);
        private Size _outputResolution = new Size(854, 480);
        private double _zoomTopPadding = 0.5;
        private double _zoomTotalHeight = 3.5;
        private int _smoothXOffset = 100;
        private int _smoothXSpeed = 5;
        private int _smoothYOffset = 70;
        private int _smoothYSpeed = 5;
        private int _smoothZOffset = 20;
        private int _smoothZSpeed = 2;
        private Size _minFaceSize = new Size(100, 100);

        private void LoadOBSConfig()
        {
            var xml = XDocument.Load(xmlFile);
            var query = from c in xml.Root.Descendants("Config")
                        select c;
            foreach (var item in query)
            {
                switch ((string)item.Attribute("Key"))
                {
                    case "ProcessName":
                        _obsProcess = (string)item.Attribute("Value");
                        break;
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

                    case "CameraName":
                        _cameraName = (string)item.Attribute("Value");
                        break;
                    case "DetectSpeed":
                        _detectSpeed = (int)item.Attribute("Value");
                        break;
                    case "IgnoreRegion":
                        _ignoreTop = (int)item.Attribute("Top");
                        _ignoreBottom = (int)item.Attribute("Bottom");
                        break;
                    case "MainArea":
                        _detectMainArea = new Size((int)item.Attribute("Width"), (int)item.Attribute("Height"));
                        _detectOutsideSec = (int)item.Attribute("Outside");
                        break;
                    case "InputResolution":
                        _inputResolution = new Size((int)item.Attribute("Width"), (int)item.Attribute("Height"));
                        break;
                    case "OutputResolution":
                        _outputResolution = new Size((int)item.Attribute("Width"), (int)item.Attribute("Height"));
                        break;
                    case "Zoom":
                        _zoomTopPadding = (double)item.Attribute("TopPadding");
                        _zoomTotalHeight = (double)item.Attribute("TotalHeight");
                        break;
                    case "Smoothing":
                        _smoothXOffset = (int)item.Attribute("XOffset");
                        _smoothXSpeed = (int)item.Attribute("XSpeed");
                        _smoothYOffset = (int)item.Attribute("YOffset");
                        _smoothYSpeed = (int)item.Attribute("YSpeed");
                        _smoothZOffset = (int)item.Attribute("ZOffset");
                        _smoothZSpeed = (int)item.Attribute("ZSpeed");
                        break;
                    case "MinFaceSize":
                        _minFaceSize = new Size((int)item.Attribute("Width"), (int)item.Attribute("Height"));
                        break;
                }
            }
        }
        public string getOBSProcessName()
        {
            return _obsProcess;
        }
        public string getOBSHotkey(string action)
        {
            switch(action)
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

        public string getCameraName()
        {
            if (Program.DEBUG)
                return "AVerMedia ExtremeCap SDI UVC";
            return _cameraName;
        }
        public int getDetectSpeed()
        {
            return _detectSpeed;
        }
        public int getTopIgnorePercent()
        {
            return _ignoreTop;
        }
        public int getBottomIgnorePercent()
        {
            return _ignoreBottom;
        }
        public Size getDetectMainArea()
        {
            if (Program.DEBUG)
                return new Size(400, 400);
            return _detectMainArea;
        }
        public int getDetectOutsideWaitSeconds()
        {
            return _detectOutsideSec;
        }
        public Size getCameraInputResolution()
        {
            if (Program.DEBUG)
                return new Size(1920, 1080);
            return _inputResolution;
        }
        public Size getOutputOutputResolution()
        {
            return _outputResolution;
        }
        public double getZoomTopPadding()
        {
            return _zoomTopPadding;
        }
        public double getZoomTotalHeight()
        {
            return _zoomTotalHeight;
        }
        public int getSmoothingXOffset()
        {
            return _smoothXOffset;
        }
        public int getSmoothingXSpeed()
        {
            return _smoothXSpeed;
        }
        public int getSmoothingYOffset()
        {
            return _smoothYOffset;
        }
        public int getSmoothingYSpeed()
        {
            return _smoothYSpeed;
        }
        public int getSmoothingZOffset()
        {
            return _smoothZOffset;
        }
        public int getSmoothingZSpeed()
        {
            return _smoothZSpeed;
        }
        public int getMinFaceWidth()
        {
            return _minFaceSize.Width;
        }
        public int getMinFaceHeight()
        {
            return _minFaceSize.Height;
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
    }
}
