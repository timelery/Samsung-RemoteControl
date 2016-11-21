using System;
using System.Runtime.InteropServices;

namespace SmartTVRemoteControl.Native
{
	[Guid("BFB7FF88-7239-4FC9-8FA2-07C950BE9C6D")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioSessionControl2
	{
		int GetDisplayName(out string pRetVal);

		int GetGroupingParam(out Guid pRetVal);

		int GetIconPath(out string pRetVal);

		int GetProcessId(out ulong pRetVal);

		int GetSessionIdentifier(out string pRetVal);

		int GetSessionInstanceIdentifier(out string pRetVal);

		int GetState();

		int IsSystemSoundsSession();

		int RegisterAudioSessionNotification();

		int SetDisplayName(string Value, ref Guid EventContext);

		int SetDuckingPreference(bool optOut);

		int SetGroupingParam(ref Guid Override, ref Guid EventContext);

		int SetIconPath(string Value, ref Guid EventContext);

		int UnregisterAudioSessionNotification();
	}
}