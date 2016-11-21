using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace SmartTVRemoteControl.Native.MultiScreen
{
	[JsonObject(MemberSerialization.OptIn, Id="mediaQueueItem")]
	public class MediaQueueItem
	{
		[JsonProperty("id")]
		public string Id
		{
			get;
			set;
		}

		[JsonProperty("index")]
		public int Index
		{
			get;
			set;
		}

		[JsonProperty("media")]
		public MediaContent Media
		{
			get;
			set;
		}

		public MediaQueueItem()
		{
		}
	}
}