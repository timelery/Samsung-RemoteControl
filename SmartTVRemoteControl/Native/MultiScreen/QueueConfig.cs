using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace SmartTVRemoteControl.Native.MultiScreen
{
	[JsonObject(MemberSerialization.OptIn, Id="config")]
	public class QueueConfig
	{
		[JsonProperty("effect")]
		public string Effect
		{
			get;
			set;
		}

		[JsonProperty("imageDisplayingTime")]
		public int ImageDisplayingTime
		{
			get;
			set;
		}

		[JsonProperty("loop")]
		public bool Loop
		{
			get;
			set;
		}

		[JsonProperty("speed")]
		public string Speed
		{
			get;
			set;
		}

		public QueueConfig()
		{
		}
	}
}