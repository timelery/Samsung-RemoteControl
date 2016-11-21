using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace SmartTVRemoteControl.Native.MultiScreen
{
	[JsonObject(MemberSerialization.OptIn, Id="mediaPanel")]
	public class MediaPanel
	{
		[JsonProperty("currentMedia")]
		public MediaContent CurrentMedia
		{
			get;
			set;
		}

		[JsonProperty("currentPanelItem")]
		public MediaContent CurrentPanelItem
		{
			get;
			set;
		}

		public MediaPanel()
		{
		}
	}
}