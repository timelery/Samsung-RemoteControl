using SmartView2.Core;
using SmartTVRemoteControl.Properties;
using System;
using System.Collections.Specialized;
using System.Configuration;

namespace SmartTVRemoteControl
{
	public class DeviceSettingProvider : IDeviceSettingProvider
	{
		public DeviceSettingProvider()
		{
		}

		public string LoadLastIp()
		{
			return Settings.Default.LastConnectedDeviceAddress;
		}

		public string LoadPin(string lastConnectedDeviceAddress)
		{
			string str;
			if (Settings.Default.ConnectedDeviceAddresses == null)
			{
				Settings.Default.ConnectedDeviceAddresses = new StringCollection();
			}
			else
			{
				StringEnumerator enumerator = Settings.Default.ConnectedDeviceAddresses.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						if (!current.Contains(lastConnectedDeviceAddress))
						{
							continue;
						}
						string[] strArrays = current.Split(new char[] { ':' });
						if ((int)strArrays.Length != 2)
						{
							continue;
						}
						str = strArrays[1];
						return str;
					}
					return "";
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				return str;
			}
			return "";
		}

		public void ResetLastConnectionAddress()
		{
			Settings.Default.LastConnectedDeviceAddress = string.Empty;
			Settings.Default.Save();
		}

		public void Save(string lastConnectedDeviceAddress, string lastConnectedDevicePin)
		{
			if (Settings.Default.ConnectedDeviceAddresses == null)
			{
				Settings.Default.ConnectedDeviceAddresses = new StringCollection();
			}
			else
			{
				foreach (string connectedDeviceAddress in Settings.Default.ConnectedDeviceAddresses)
				{
					if (!connectedDeviceAddress.Contains(lastConnectedDeviceAddress))
					{
						continue;
					}
					Settings.Default.ConnectedDeviceAddresses.Remove(connectedDeviceAddress);
					break;
				}
			}
			if (string.IsNullOrEmpty(lastConnectedDevicePin))
			{
				Settings.Default.LastConnectedDeviceAddress = string.Empty;
			}
			else
			{
				Settings.Default.ConnectedDeviceAddresses.Add(string.Format("{0}:{1}", lastConnectedDeviceAddress, lastConnectedDevicePin));
				Settings.Default.LastConnectedDeviceAddress = lastConnectedDeviceAddress;
			}
			Settings.Default.Save();
		}
	}
}