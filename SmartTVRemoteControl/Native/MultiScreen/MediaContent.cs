using Newtonsoft.Json;
using SmartView2.Core;
using System;
using System.Runtime.CompilerServices;

namespace SmartTVRemoteControl.Native.MultiScreen
{
	[JsonObject(MemberSerialization.OptIn, Id="media")]
	public class MediaContent : IMediaContent
	{
		[JsonProperty("albumName")]
		public string AlbumName
		{
			get;
			set;
		}

		[JsonProperty("artistName")]
		public string ArtistName
		{
			get;
			set;
		}

		[JsonProperty("id")]
		public string Id
		{
			get;
			set;
		}

		[JsonProperty("name")]
		public string Name
		{
			get;
			set;
		}

		[JsonProperty("source")]
		public Uri Source
		{
			get;
			set;
		}

		[JsonProperty("thumbnail")]
		public Uri Thumbnail
		{
			get;
			set;
		}

		[JsonProperty("type")]
		public string Type
		{
			get;
			set;
		}

		[JsonProperty("user")]
		public IMediaUser User
		{
			get;
			set;
		}

		public MediaContent()
		{
			this.User = new MediaUser();
		}
	}
}