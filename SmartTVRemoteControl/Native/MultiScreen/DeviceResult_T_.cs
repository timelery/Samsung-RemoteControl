using com.samsung.multiscreen.device;
using System;
using System.Threading;

namespace SmartTVRemoteControl.Native.MultiScreen
{
	internal class DeviceResult<T> : DeviceAsyncResult<T>
	{
		public DeviceResult()
		{
		}

		public void OnError(DeviceError error)
		{
			EventHandler<DeviceError> eventHandler = this.OnFailed;
			if (eventHandler != null)
			{
				eventHandler(this, error);
			}
		}

		public void OnResult(T result)
		{
			EventHandler<T> eventHandler = this.OnSuccess;
			if (eventHandler != null)
			{
				eventHandler(this, result);
			}
		}

		public event EventHandler<DeviceError> OnFailed;

		public event EventHandler<T> OnSuccess;
	}
}