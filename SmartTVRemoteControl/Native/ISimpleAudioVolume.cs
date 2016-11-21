using System;
using System.Runtime.InteropServices;

namespace SmartTVRemoteControl.Native
{
	[Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface ISimpleAudioVolume
	{
		int GetMasterVolume(out float pfLevel);

		int GetMute(out bool pbMute);

		int SetMasterVolume(float fLevel, ref Guid EventContext);

		int SetMute(bool bMute, ref Guid EventContext);
	}
}