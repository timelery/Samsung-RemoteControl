using SmartTVRemoteControl.Native.HTTP;
using SmartTVRemoteControl.Properties;
using System;
using System.Runtime.CompilerServices;

namespace SmartTVRemoteControl.Native.DLNA.Handler
{
	internal class ContentDirectoryHandler : IHttpHandler
	{
		public string Prefix
		{
			get
			{
				return JustDecompileGenerated_get_Prefix();
			}
			set
			{
				JustDecompileGenerated_set_Prefix(value);
			}
		}

		private string JustDecompileGenerated_Prefix_k__BackingField;

		public string JustDecompileGenerated_get_Prefix()
		{
			return this.JustDecompileGenerated_Prefix_k__BackingField;
		}

		private void JustDecompileGenerated_set_Prefix(string value)
		{
			this.JustDecompileGenerated_Prefix_k__BackingField = value;
		}

		public ContentDirectoryHandler()
		{
			this.Prefix = "/contentDirectory.xml";
		}

		public HttpResponse HandleRequest(HttpRequest request)
		{
			return new HttpResponse(request, HttpCode.Ok, "text/xml", "C:\\Users\\Tim\\Desktop\\content");
		}
	}
}