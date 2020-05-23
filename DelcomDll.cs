using System;
using System.Text;
using System.Runtime.InteropServices;

public class Delcom {
    // Delcom USB Devices
    public const uint USBIODS = 1;
    public const uint USBDELVI = 2;
    public const uint USBNDSPY = 3;

    // USBDELVI LED MODES
    public const byte LEDOFF = 0;
    public const byte LEDON = 1;
    public const byte LEDFLASH = 2;

    // USBDELVI LED COlORS
    public const byte GREENLED = 0;
    public const byte REDLED = 1;
    public const byte BLUELED = 2;

    // Device Name Maximum Length
    public const int MAXDEVICENAMELEN = 512;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DeviceNameStruct
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXDEVICENAMELEN)]
        public byte[] DeviceName;
    }

    [DllImport("delcomdll.dll", EntryPoint="DelcomGetDLLVersion")]
    public static extern float DelcomGetDLLVersion();
    

    // Sets the verbose controll - used for debugging
    [DllImport("delcomdll.dll", EntryPoint="DelcomVerboseControl")]
    public static extern int DelcomVerboseControl(uint Mode, StringBuilder caption);
    

    // Gets the DLL date
    [DllImport("delcomdll.dll", EntryPoint="DelcomGetDLLDate")]
    public static extern int DelcomGetDLLDate(StringBuilder DateString);


    // Generic Functions

    // Gets Nth Device
    [DllImport("delcomdll.dll", EntryPoint = "DelcomScanDevices")]
    public static extern int DelcomScanDevices(uint ProductType, DeviceNameStruct[] DeviceNames, uint MAX);

    //Gets DeviceCount
    [DllImport("delcomdll.dll", EntryPoint="DelcomGetDeviceCount")]
    public static extern int DelcomGetDeviceCount(uint ProductType);
    
    // Gets Nth Device
    [DllImport("delcomdll.dll", EntryPoint="DelcomGetNthDevice")]
    public static extern int DelcomGetNthDevice(uint ProductType, uint NthDevice, StringBuilder DeviceName);
    
    // Open Device
    [DllImport("delcomdll.dll", EntryPoint="DelcomOpenDevice")]
    public static extern uint DelcomOpenDevice(StringBuilder DeviceName, int Mode );
    
    // Close Device
    [DllImport("delcomdll.dll", EntryPoint="DelcomCloseDevice")]
    public static extern int DelcomCloseDevice(uint DeviceHandle);
    

    // USBDELVI - Visual Indicator Functions

    // Set LED Functions
    [DllImport("delcomdll.dll", EntryPoint="DelcomLEDControl")]
    public static extern int DelcomLEDControl(uint DeviceHandle, int Color, int Mode);
    

    // Set LED Freq/Duty functions
    [DllImport("delcomdll.dll", EntryPoint="DelcomLoadLedFreqDuty")]
    public static extern int DelcomLoadLedFreqDuty(uint DeviceHandle, byte Color, byte Low, byte High);
    

    // Set Auto Confirm Mode
    [DllImport("delcomdll.dll", EntryPoint="DelcomEnableAutoConfirm")]
    public static extern int DelcomEnableAutoConfirm(uint DeviceHandle, int Mode );
    

    // Set Auto Clear Mode
    [DllImport("delcomdll.dll", EntryPoint="DelcomEnableAutoClear(")]
    public static extern int DelcomEnableAutoClear(uint DeviceHandle, int Mode);
    


    // Set Buzzer Function
    [DllImport("delcomdll.dll", EntryPoint="DelcomBuzzer")]
    public static extern int DelcomBuzzer(uint DeviceHandle, byte  Mode , byte Freq, byte Repeat, byte OnTime, byte OffTime);
    

    // Set LED Phase Delay
    [DllImport("delcomdll.dll", EntryPoint="DelcomLoadInitialPhaseDelay")]
    public static extern int DelcomLoadInitialPhaseDelay(uint DeviceHandle, byte Color, byte Delay);
    

    // Set Led Sync Functions
    [DllImport("delcomdll.dll", EntryPoint="DelcomSyncLeds")]
    public static extern int DelcomSyncLeds(uint DeviceHandle );
    


    // Set LED PreScalar Functions
    [DllImport("delcomdll.dll", EntryPoint="DelcomLoadPreScalar")]
    public static extern int DelcomLoadPreScalar(uint DeviceHandle, byte PreScalar);
    


    // Get Button Status
    [DllImport("delcomdll.dll", EntryPoint="DelcomGetButtonStatus")]
    public static extern int DelcomGetButtonStatus(uint DeviceHandle);
    

    // Set LED Power
    [DllImport("delcomdll.dll", EntryPoint="DelcomLEDPower")]
    public static extern int DelcomLEDPower(uint DeviceHandle, int Color, int Power);
    
}


