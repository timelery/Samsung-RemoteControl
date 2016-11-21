using System;

namespace SmartTVRemoteControl.Native.Wlan
{
	public enum WlanInterfaceState
	{
		NOT_READY,
		CONNECTED,
		AD_HOC_NETWORK_FORMED,
		DISCONNECTING,
		DISCONNECTED,
		ASSOCIATING,
		DISCOVERING,
		AUTHENTICATING
	}
}