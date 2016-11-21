using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace SmartTVRemoteControl.Native.MultiScreen
{
	[JsonObject(MemberSerialization.OptIn, Id="mediaQueuePlayer")]
	public class MediaQueuePlayer
	{
		[JsonProperty("config")]
		public QueueConfig Config
		{
			get;
			set;
		}

		[JsonProperty("mediaQueue")]
		public MediaQueueItem[] MediaQueue
		{
			get;
			set;
		}

		[JsonProperty("status")]
		public string Status
		{
			get;
			set;
		}

		public MediaQueuePlayer()
		{
		}
	}
}