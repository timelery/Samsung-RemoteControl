using SmartTVRemoteControl.Native.DLNA;
using SmartTVRemoteControl.Native.HTTP;
using System;
using System.Reflection;
using System.Xml;

namespace SmartTVRemoteControl.Native.DLNA.Handler
{
	internal class DescriptionHandler : IHttpHandler
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

		public DescriptionHandler()
		{
			this.Prefix = "/description.xml";
		}

		public HttpResponse HandleRequest(HttpRequest request)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(Properties.Resources.description);
			xmlDocument.GetElementsByTagName("UDN").Item(0).InnerText = string.Format("uuid:{0}", DlnaServer.ServerGuid);
			xmlDocument.GetElementsByTagName("modelNumber").Item(0).InnerText = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			xmlDocument.GetElementsByTagName("friendlyName").Item(0).InnerText = Environment.MachineName;
			xmlDocument.GetElementsByTagName("SCPDURL").Item(0).InnerText = "/contentDirectory.xml";
			xmlDocument.GetElementsByTagName("controlURL").Item(0).InnerText = "/control";
			xmlDocument.GetElementsByTagName("eventSubURL").Item(0).InnerText = "/events";
			return new HttpResponse(request, HttpCode.Ok, "text/xml", xmlDocument.OuterXml);
		}
	}
}