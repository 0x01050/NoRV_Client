using NoRV;
using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

public class L
{
	public static string GetComputerName()
	{
		try
		{
			ManagementClass managementClass = new ManagementClass("Win32_ComputerSystem");
			ManagementObjectCollection instances = managementClass.GetInstances();
			string result = string.Empty;
			foreach (ManagementObject item in instances)
			{
				result = (string)item["Name"];
			}
			return result;
		}
		catch
		{
			return "";
		}
	}

	public static string GetOSInformation()
	{
		try
		{
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
			foreach (ManagementObject item in managementObjectSearcher.Get())
			{
				try
				{
					return ((string)item["Caption"]).Trim() + ", " + (string)item["Version"] + ", " + (string)item["OSArchitecture"];
				}
				catch
				{
				}
			}
			return "BIOS Maker: Unknown";
		}
		catch
		{
			return "";
		}
	}

	public static string GetProcessorId()
	{
		try
		{
			ManagementClass managementClass = new ManagementClass("win32_processor");
			ManagementObjectCollection instances = managementClass.GetInstances();
			string result = string.Empty;
			using (ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = instances.GetEnumerator())
			{
				if (managementObjectEnumerator.MoveNext())
				{
					ManagementObject managementObject = (ManagementObject)managementObjectEnumerator.Current;
					result = managementObject.Properties["processorID"].Value.ToString();
				}
			}
			return result;
		}
		catch
		{
			return "";
		}
	}

	public static string GetHDDSerialNo()
	{
		try
		{
			ManagementClass managementClass = new ManagementClass("Win32_LogicalDisk");
			ManagementObjectCollection instances = managementClass.GetInstances();
			string text = "";
			foreach (ManagementObject item in instances)
			{
				text += Convert.ToString(item["VolumeSerialNumber"]);
			}
			return text;
		}
		catch
		{
			return "";
		}
	}

	public static string GetMACAddress()
	{
		try
		{
			ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
			ManagementObjectCollection instances = managementClass.GetInstances();
			string text = string.Empty;
			foreach (ManagementObject item in instances)
			{
				if (text == string.Empty && (bool)item["IPEnabled"])
				{
					text = item["MacAddress"].ToString();
				}
				item.Dispose();
			}
			return text.Replace(":", "");
		}
		catch
		{
			return "";
		}
	}

	public static string GetBoardMaker()
	{
		try
		{
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
			foreach (ManagementObject item in managementObjectSearcher.Get())
			{
				try
				{
					return item.GetPropertyValue("Manufacturer").ToString();
				}
				catch
				{
				}
			}
			return "Board Maker: Unknown";
		}
		catch
		{
			return "";
		}
	}

	public static string GetBoardProductId()
	{
		try
		{
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
			foreach (ManagementObject item in managementObjectSearcher.Get())
			{
				try
				{
					return item.GetPropertyValue("Product").ToString();
				}
				catch
				{
				}
			}
			return "Product: Unknown";
		}
		catch
		{
			return "";
		}
	}

	public static string GetBIOSmaker()
	{
		try
		{
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");
			foreach (ManagementObject item in managementObjectSearcher.Get())
			{
				try
				{
					return item.GetPropertyValue("Manufacturer").ToString();
				}
				catch
				{
				}
			}
			return "BIOS Maker: Unknown";
		}
		catch
		{
			return "";
		}
	}

	public static string GetBIOSserNo()
	{
		try
		{
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");
			foreach (ManagementObject item in managementObjectSearcher.Get())
			{
				try
				{
					return item.GetPropertyValue("SerialNumber").ToString();
				}
				catch
				{
				}
			}
			return "BIOS Serial Number: Unknown";
		}
		catch
		{
			return "";
		}
	}

	public static string GetBIOScaption()
	{
		try
		{
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");
			foreach (ManagementObject item in managementObjectSearcher.Get())
			{
				try
				{
					return item.GetPropertyValue("Caption").ToString();
				}
				catch
				{
				}
			}
			return "BIOS Caption: Unknown";
		}
		catch
		{
			return "";
		}
	}

	public static string GetAccountName()
	{
		try
		{
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_UserAccount");
			foreach (ManagementObject item in managementObjectSearcher.Get())
			{
				try
				{
					return item.GetPropertyValue("Name").ToString();
				}
				catch
				{
				}
			}
			return "User Account Name: Unknown";
		}
		catch
		{
			return "";
		}
	}

	public static string GetPhysicalMemory()
	{
		try
		{
			ManagementScope scope = new ManagementScope();
			ObjectQuery query = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, query);
			ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
			long num = 0L;
			long num2 = 0L;
			foreach (ManagementObject item in managementObjectCollection)
			{
				num2 = Convert.ToInt64(item["Capacity"]);
				num += num2;
			}
			return (num / 1024 / 1024).ToString() + "MB";
		}
		catch
		{
			return "";
		}
	}

	public static string GetNoRamSlots()
	{
		try
		{
			int num = 0;
			ManagementScope scope = new ManagementScope();
			ObjectQuery query = new ObjectQuery("SELECT MemoryDevices FROM Win32_PhysicalMemoryArray");
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, query);
			ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
			foreach (ManagementObject item in managementObjectCollection)
			{
				num = Convert.ToInt32(item["MemoryDevices"]);
			}
			return num.ToString();
		}
		catch
		{
			return "";
		}
	}

	public static string GetCPUManufacturer()
	{
		try
		{
			string text = string.Empty;
			ManagementClass managementClass = new ManagementClass("Win32_Processor");
			ManagementObjectCollection instances = managementClass.GetInstances();
			foreach (ManagementObject item in instances)
			{
				if (text == string.Empty)
				{
					text = item.Properties["Manufacturer"].Value.ToString();
				}
			}
			return text;
		}
		catch
		{
			return "";
		}
	}

	public static string GetDefaultIPGateway()
	{
		try
		{
			ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
			ManagementObjectCollection instances = managementClass.GetInstances();
			string text = string.Empty;
			foreach (ManagementObject item in instances)
			{
				if (text == string.Empty && (bool)item["IPEnabled"])
				{
					text = item["DefaultIPGateway"].ToString();
				}
				item.Dispose();
			}
			return text.Replace(":", "");
		}
		catch
		{
			return "";
		}
	}

	private static string ComputeSha256Hash(string rawData)
	{
		using (SHA256 sHA = SHA256.Create())
		{
			byte[] array = sHA.ComputeHash(Encoding.UTF8.GetBytes(rawData));
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}
	}

	public static string g()
	{
		string rawData = GetHDDSerialNo();
		return ComputeSha256Hash(ComputeSha256Hash(rawData));
	}

	public static string s(string v)
	{
		v += "hdjeirotuhg7485gh5739fn3h350fm3";
		for (int i = 0; i < 26; i++)
		{
			v = ComputeSha256Hash(v);
		}
		return v;
	}

	public static string w(string t, string k)
	{
		string sharedSecret = s(k);
		return ClassAES.EncryptStringAES(t, sharedSecret);
	}

    private static string serial = "";

    public static string v()
	{
		try
		{
            if (string.IsNullOrEmpty(serial))
			{
				serial = g();
			}
			return serial;
		}
		catch (Exception)
		{
			throw new SystemException("Cannot Get NoRV Machine ID");
		}
	}

	private static string machineID = "";
	public static string getID()
    {
		return machineID;
    }
	public static void setID(string id)
    {
		machineID = id;
    }
}
