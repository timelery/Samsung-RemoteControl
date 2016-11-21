using MediaLibrary.DataModels;
using SmartView2.Core;
using SmartTVRemoteControl.Native.DLNA;
using SmartTVRemoteControl.Native.HTTP;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SmartTVRemoteControl.Native.DLNA.Handler
{
	internal class FileHandler : IHttpHandler
	{
		private readonly IDataLibrary dataLibrary;

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

		public FileHandler(IDataLibrary dataLibrary)
		{
			this.dataLibrary = dataLibrary;
			this.Prefix = "/file/*";
		}

		public HttpResponse HandleRequest(HttpRequest request)
		{
			string str = request.Path.Substring(this.Prefix.Length - 1);
			Content itemById = this.dataLibrary.GetItemById(new Guid(str), this.dataLibrary.RootFolder) as Content;
			if (itemById == null)
			{
				return new HttpResponse(request, HttpCode.NotFound, "text/html", "");
			}
			string mimeByFileType = MimeTypeSolver.GetMimeByFileType(Path.GetExtension(itemById.Path));
			HttpResponse httpResponse = new HttpResponse(request, HttpCode.Ok, mimeByFileType, new FileStream(itemById.Path, FileMode.Open, FileAccess.Read));
			httpResponse.Headers["Accept-Ranges"] = "bytes";
			httpResponse.Headers["transferMode.dlna.org"] = "Streaming";
			if (request.Headers.ContainsKey("getcontentFeatures.dlna.org"))
			{
				if (!mimeByFileType.StartsWith("image"))
				{
					httpResponse.Headers["contentFeatures.dlna.org"] = string.Format("{0};DLNA.ORG_OP=01;DLNA.ORG_CI=0;DLNA.ORG_FLAGS={1}", DLNAMaps.GetPNByMime(mimeByFileType), DLNAMaps.DefaultStreaming);
				}
				else
				{
					httpResponse.Headers["contentFeatures.dlna.org"] = string.Format("{0};DLNA.ORG_OP=00;DLNA.ORG_CI=0;DLNA.ORG_FLAGS={1}", DLNAMaps.GetPNByMime(mimeByFileType), DLNAMaps.DefaultInteractive);
				}
			}
			return httpResponse;
		}
	}
}