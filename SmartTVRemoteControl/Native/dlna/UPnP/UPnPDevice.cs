using System;
using System.Net;

namespace SmartTVRemoteControl.Native.DLNA.UPnP
{
	internal sealed class UPnPDevice
	{
		public readonly IPAddress Address;

		public readonly Uri Descriptor;

		public readonly string Type;

		public readonly string USN;

		public readonly Guid Uuid;

		public UPnPDevice(Guid uuid, string type, Uri descriptor, IPAddress address)
		{
			this.Uuid = uuid;
			this.Type = type;
			this.Descriptor = descriptor;
			this.Address = address;
			this.USN = (this.Type.StartsWith("uuid:") ? this.Type : string.Format("uuid:{0}::{1}", this.Uuid, this.Type));
		}
	}
}