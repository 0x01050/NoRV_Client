﻿using Google.Rpc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoRV
{
    class ButtonManager
    {
        private static ButtonManager _instance = null;
        public static ButtonManager getInstance()
        {
            if(_instance == null)
            {
                _instance = new ButtonManager();
            }
            return _instance;
        }

        public static string NO_BUTTON = "No Button";
        public static string OPEN_FAILED = "Open Failed";
        public static string INITIATED = "Initiated";

        private uint deviceHandle = 0;
        private string buttonStatus = "";

        ButtonManager()
        {
            startButton();
        }
        ~ButtonManager()
        {
            stopButton();
        }

        public void startButton()
        {
            stopButton();

            StringBuilder DeviceName = new StringBuilder(Delcom.MAXDEVICENAMELEN);
            if (Delcom.DelcomGetNthDevice(0, 0, DeviceName) == 0)
            {
                buttonStatus = NO_BUTTON;
                return;
            }
            deviceHandle = Delcom.DelcomOpenDevice(DeviceName, 0);
            if (deviceHandle == 0)
            {
                buttonStatus = OPEN_FAILED;
                return;
            }
            buttonStatus = INITIATED;
        }
        public void stopButton()
        {
            if (deviceHandle != 0)
            {
                Delcom.DelcomCloseDevice(deviceHandle);
                deviceHandle = 0;
                buttonStatus = "Closed";
            }
        }
        public string getButtonStatus()
        {
            return buttonStatus;
        }
        public void turnOffLED()
        {
            if(getButtonStatus() != INITIATED)
            {
                return;
            }

            for(int ledColor = 0; ledColor < 3; ledColor ++)
            {
                Delcom.DelcomLEDControl(deviceHandle, ledColor, Delcom.LEDON);
                Delcom.DelcomLEDPower(deviceHandle, ledColor, 0);
            }
        }
        public void turnOnLED()
        {
            if (getButtonStatus() != INITIATED)
            {
                return;
            }

            for (int ledColor = 0; ledColor < 3; ledColor++)
            {
                Delcom.DelcomLEDControl(deviceHandle, ledColor, Delcom.LEDON);
                Delcom.DelcomLEDPower(deviceHandle, ledColor, 100);
            }
        }

        public bool checkButtonPressed()
        {
            //return Control.ModifierKeys == Keys.Control;

            if (getButtonStatus() != INITIATED)
            {
                return false;
            }
            return Delcom.DelcomGetButtonStatus(deviceHandle) == 1;
        }
    }
}