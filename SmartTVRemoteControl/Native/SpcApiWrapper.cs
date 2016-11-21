using Pairing;
using SmartView2.Devices;
using System;

namespace SmartTVRemoteControl.Native
{
	public class SpcApiWrapper : ISpcApi, IDisposable
	{
		private SPCApiBridge wrapped;

		public SpcApiWrapper()
		{
		}

		public void Dispose()
		{
			if (this.wrapped != null)
			{
				this.wrapped.Dispose();
			}
		}

		public string GenerateServerAcknowledge()
		{
			return this.wrapped.GenerateServerAck();
		}

		public string GenerateServerHello(string pin)
		{
			return this.wrapped.GenerateServerHello(pin);
		}

		public byte[] GetKey()
		{
			return this.wrapped.GetKey();
		}

		public void Initialize(string userId)
		{
			if (this.wrapped != null)
			{
				this.wrapped.Dispose();
			}
			this.wrapped = new SPCApiBridge(userId);
		}

		public bool ParseClientAcknowledge(string clientAcknowledge)
		{
			return this.wrapped.ParseClientAck(clientAcknowledge);
		}

		public bool ParseClientHello(string pin, string clientHello)
		{
			return this.wrapped.ParseClientHello(pin, clientHello);
		}
	}
}