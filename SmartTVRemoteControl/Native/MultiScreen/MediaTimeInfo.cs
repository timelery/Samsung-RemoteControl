using Newtonsoft.Json;
using SmartView2.Core;
using System;
using System.Runtime.CompilerServices;

namespace SmartTVRemoteControl.Native.MultiScreen
{
	[JsonObject(MemberSerialization.OptIn, Id="timeupdate")]
	public class MediaTimeInfo : IMediaTimeInfo
	{
		[JsonProperty("currentTime")]
		public double CurrentTime
		{
			get;
			set;
		}

		[JsonProperty("duration")]
		public double Duration
		{
			get;
			set;
		}

		[JsonProperty("media")]
		public IMediaContent Media
		{
			get;
			set;
		}

		public MediaTimeInfo()
		{
			this.Media = new MediaContent();
		}
	}
}