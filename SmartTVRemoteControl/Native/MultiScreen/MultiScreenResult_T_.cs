using com.samsung.multiscreen.application;
using System;
using System.Threading;

namespace SmartTVRemoteControl.Native.MultiScreen
{
	internal class MultiScreenResult<T> : ApplicationAsyncResult<T>
	{
		public MultiScreenResult()
		{
		}

		public void OnError(ApplicationError error)
		{
			EventHandler<ApplicationError> eventHandler = this.OnFailed;
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

		public event EventHandler<ApplicationError> OnFailed;

		public event EventHandler<T> OnSuccess;
	}
}