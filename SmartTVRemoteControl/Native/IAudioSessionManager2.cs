using System;
using System.Runtime.InteropServices;

namespace SmartTVRemoteControl.Native
{
	[Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioSessionManager2
	{
		int GetAudioSessionControl();

		int GetSessionEnumerator(out IAudioSessionEnumerator SessionEnum);

		int GetSimpleAudioVolume();
	}
}