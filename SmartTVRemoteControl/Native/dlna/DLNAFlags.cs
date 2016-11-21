using System;

namespace SmartTVRemoteControl.Native.DLNA
{
	[Flags]
	internal enum DLNAFlags : ulong
	{
		DlnaV15 = 1048576,
		ConnectionStall = 2097152,
		BackgroundTransferMode = 4194304,
		InteractiveTransferMode = 8388608,
		StreamingTransferMode = 16777216,
		RtspPause = 33554432,
		SnIncrease = 67108864,
		S0Increase = 134217728,
		PlayContainer = 268435456,
		ByteBasedSeek = 536870912,
		TimeBasedSeek = 1073741824,
		SenderPaced = 2147483648
	}
}