using System;
using System.Runtime.InteropServices;

namespace SmartTVRemoteControl.Native
{
	[Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioSessionEnumerator
	{
		int GetCount(out int SessionCount);

		int GetSession(int SessionCount, out IAudioSessionControl2 Session);
	}
}