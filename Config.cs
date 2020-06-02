using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NoRV
{
    class Config
    {
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
        private List<string> _keyList = new List<string>();
        private List<string> _nameList = new List<string>();
        private void LoadMappingConfig()
        {
            var xml = XDocument.Load(@"Config.xml");
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
                    _keyList.Add((string)item.Attribute("Key"));
                    _nameList.Add((string)item.Attribute("Name"));
                }
            }
        }
        public string getMachineIDKey()
        {
            return _machineIDKey;
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
        private int _defaultVolume = 65;
        private string _announceTime = "";
        private int _flashPeriod = 1000;
        private void LoadGlobalConfig()
        {
            var xml = XDocument.Load(@"Config.xml");
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


        // OBS Config
        private string _obsProcess = "obs64";
        private string _startHotkey = "^R";
        private string _stopHotkey = "^S";
        private string _pauseHotkey = "^P";
        private string _unpauseHotkey = "^U";
        private void LoadOBSConfig()
        {
            var xml = XDocument.Load(@"Config.xml");
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
            }
            return "";
        }


        public string getTemplate(string template)
        {
            var xml = XDocument.Load(@"Config.xml");
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
