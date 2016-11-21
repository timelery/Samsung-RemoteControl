using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace SmartTVRemoteControl.Native.DLNA.SSDP
{
	internal sealed class Datagram
	{
		public readonly IPEndPoint EndPoint;

		public readonly IPAddress LocalAddress;

		public readonly string Message;

		public readonly bool Sticky;

		public uint SendCount
		{
			get;
			private set;
		}

		public Datagram(IPEndPoint endPoint, IPAddress localAddresss, string message, bool sticky)
		{
			this.EndPoint = endPoint;
			this.LocalAddress = localAddresss;
			this.Message = message;
			this.Sticky = sticky;
			this.SendCount = 0;
		}

		public void Send()
		{
			byte[] bytes = Encoding.ASCII.GetBytes(this.Message);
			try
			{
				UdpClient udpClient = new UdpClient();
				udpClient.Client.Bind(new IPEndPoint(this.LocalAddress, 0));
				udpClient.BeginSend(bytes, (int)bytes.Length, this.EndPoint, (IAsyncResult result) => {
					try
					{
						try
						{
							udpClient.EndSend(result);
						}
						catch (Exception exception)
						{
						}
					}
					finally
					{
						try
						{
							udpClient.Close();
						}
						catch (Exception exception1)
						{
						}
					}
				}, null);
			}
			catch (Exception exception2)
			{
			}
			Datagram sendCount = this;
			sendCount.SendCount = sendCount.SendCount + 1;
		}
	}
}