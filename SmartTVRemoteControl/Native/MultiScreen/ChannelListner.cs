using com.samsung.multiscreen.channel;
using System;
using System.Threading;

namespace SmartTVRemoteControl.Native.MultiScreen
{
	public class ChannelListner : IChannelListener
	{
		public ChannelListner()
		{
		}

		public void OnClientConnected(ChannelClient client)
		{
		}

		public void OnClientDisconnected(ChannelClient client)
		{
		}

		public void OnClientMessage(ChannelClient client, string message)
		{
			EventHandler<string> eventHandler = this.MessageReceived;
			if (eventHandler != null)
			{
				eventHandler(this, message);
			}
		}

		public void OnConnect()
		{
		}

		public void OnDisconnect()
		{
			EventHandler eventHandler = this.DisconnectReceived;
			if (eventHandler != null)
			{
				eventHandler(this, new EventArgs());
			}
		}

		public event EventHandler DisconnectReceived;

		public event EventHandler<string> MessageReceived;
	}
}