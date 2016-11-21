using System;
using System.Runtime.CompilerServices;

namespace SmartTVRemoteControl.Native.Wlan
{
	public sealed class WlanConnectionInfo
	{
		public WlanConnectionMode ConnectionMode
		{
			get;
			internal set;
		}

		public string ProfileName
		{
			get;
			internal set;
		}

		public WlanInterfaceState State
		{
			get;
			internal set;
		}

		public WlanConnectionInfo()
		{
		}
	}
}