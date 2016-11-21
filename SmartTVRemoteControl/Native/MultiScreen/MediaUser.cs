using Newtonsoft.Json;
using SmartView2.Core;
using System;
using System.Runtime.CompilerServices;

namespace SmartTVRemoteControl.Native.MultiScreen
{
	[JsonObject(MemberSerialization.OptIn, Id="user")]
	public class MediaUser : IMediaUser
	{
		[JsonProperty("id")]
		public Guid Id
		{
			get;
			set;
		}

		[JsonProperty("serverID")]
		public Guid ServerID
		{
			get;
			set;
		}

		public MediaUser()
		{
		}
	}
}