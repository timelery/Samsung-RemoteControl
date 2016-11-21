using MediaLibrary.DataModels;
using SmartView2.Core;
using SmartTVRemoteControl.Native.DLNA;
using SmartTVRemoteControl.Native.HTTP;
using SmartTVRemoteControl.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace SmartTVRemoteControl.Native.DLNA.Handler
{
	internal class ControlHandler : IHttpHandler
	{
		private XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";

		private XNamespace u = "urn:schemas-upnp-org:metadata-1-0/upnp/";

		private XNamespace didl = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/";

		private XNamespace dc = "http://purl.org/dc/elements/1.1/";

		private XNamespace dlna = "urn:schemas-dlna-org:metadata-1-0/";

		private XNamespace upnp = "urn:schemas-upnp-org:metadata-1-0/upnp/";

		private XNamespace sec = "http://www.sec.co.kr/";

		private uint systemID = 1;

		private IHttpServer httpServer;

		private IDataLibrary dataLibrary;

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

		public ControlHandler(IHttpServer httpServer, IDataLibrary dataLibrary)
		{
			this.httpServer = httpServer;
			this.dataLibrary = dataLibrary;
			this.Prefix = "/control";
		}

		private XElement CreateFolder(ItemBase resource)
		{
			MultimediaFolder multimediaFolder = resource as MultimediaFolder;
			XName xName = this.didl + "container";
			object[] xAttribute = new object[] { new XAttribute("restricted", (object)0), new XAttribute("childCount", (object)multimediaFolder.ItemsList.Count), new XAttribute("id", (object)multimediaFolder.ID), null };
			xAttribute[3] = new XAttribute("parentID", (multimediaFolder.Parent == Guid.Empty ? "0" : multimediaFolder.Parent.ToString()));
			XElement xElement = new XElement(xName, xAttribute);
			xElement.Add(new XElement(this.dc + "title", multimediaFolder.Name));
			XName xName1 = this.dc + "date";
			DateTime date = multimediaFolder.Date;
			xElement.Add(new XElement(xName1, date.ToString("o")));
			xElement.Add(new XElement(this.upnp + "class", "object.container"));
			return xElement;
		}

		private XElement CreateItem(ItemBase resource)
		{
			Content content = resource as Content;
			XName xName = this.didl + "item";
			object[] xAttribute = new object[] { new XAttribute("restricted", (object)1), new XAttribute("id", (object)resource.ID), new XAttribute("parentID", (object)content.Parent) };
			XElement xElement = new XElement(xName, xAttribute);
			xElement.Add(this.CreateObjectClass(resource));
			xElement.Add(new XElement(this.dc + "title", resource.Name));
			XName xName1 = this.dc + "date";
			DateTime date = content.Date;
			xElement.Add(new XElement(xName1, date.ToString("o")));
			Track track = content as Track;
			if (track != null)
			{
				xElement.Add(new XElement(this.upnp + "genre", track.Genre.Name));
				xElement.Add(new XElement(this.upnp + "artist", track.Artist.Name));
				xElement.Add(new XElement(this.upnp + "album", track.Album.Name));
				if (track.Preview != null && File.Exists(track.Preview.AbsolutePath))
				{
					XName xName2 = this.upnp + "albumArtURI";
					object[] objArray = new object[] { new XAttribute(this.dlna + "profileID", "JPEG_TN"), string.Format("{0}/cover/{1}", this.httpServer.ServerUrl, resource.ID) };
					xElement.Add(new XElement(xName2, objArray));
				}
			}
			string mimeByFileType = MimeTypeSolver.GetMimeByFileType(Path.GetExtension(content.Path));
			XName xName3 = this.didl + "res";
			object[] xAttribute1 = new object[] { new XAttribute("size", (object)(new FileInfo(content.Path)).Length), new XAttribute("protocolInfo", string.Format("http-get:*:{1}:{0};DLNA.ORG_OP=01;DLNA.ORG_OP=01;DLNA.ORG_FLAGS={2}", DLNAMaps.GetPNByMime(mimeByFileType), mimeByFileType, DLNAMaps.DefaultStreaming)), string.Format("{0}/file/{1}", this.httpServer.ServerUrl, resource.ID) };
			xElement.Add(new XElement(xName3, xAttribute1));
			return xElement;
		}

		private XElement CreateObjectClass(ItemBase resource)
		{
			XElement xElement = new XElement(this.upnp + "class");
			switch ((resource as Content).ContentType)
			{
				case ContentType.Image:
				{
					xElement.Value = "object.item.imageItem.photo";
					break;
				}
				case ContentType.Track:
				{
					xElement.Value = "object.item.audioItem.musicTrack";
					break;
				}
				case ContentType.Video:
				{
					xElement.Value = "object.item.videoItem.movie";
					break;
				}
				default:
				{
					throw new NotSupportedException();
				}
			}
			return xElement;
		}

		private IEnumerable<XElement> HandleBrowse(HttpRequest request, Dictionary<string, string> sparams)
		{
			int num;
			int num1;
			string item = sparams["ObjectID"];
			string str = sparams["BrowseFlag"];
			if (int.TryParse(sparams["RequestedCount"], out num) && num <= 0)
			{
				num = 20;
			}
			if (int.TryParse(sparams["StartingIndex"], out num1) && num1 <= 0)
			{
				num1 = 0;
			}
			MultimediaFolder rootFolder = null;
			string str1 = item;
			string str2 = str1;
			if (str1 == null)
			{
				goto Label0;
			}
			else if (str2 == "0")
			{
				rootFolder = this.dataLibrary.RootFolder;
			}
			else if (str2 == "I")
			{
				rootFolder = this.dataLibrary.RootImageFolder;
			}
			else if (str2 == "A")
			{
				rootFolder = this.dataLibrary.RootMusicFolder;
			}
			else
			{
				if (str2 != "V")
				{
					goto Label0;
				}
				rootFolder = this.dataLibrary.RootVideoFolder;
			}
		Label2:
			XName xName = this.didl + "DIDL-Lite";
			object[] xAttribute = new object[] { new XAttribute(XNamespace.Xmlns + "dc", this.dc), new XAttribute(XNamespace.Xmlns + "dlna", this.dlna), new XAttribute(XNamespace.Xmlns + "upnp", this.upnp), new XAttribute(XNamespace.Xmlns + "sec", this.sec) };
			XElement xElement = new XElement(xName, xAttribute);
			if (str != "BrowseMetadata")
			{
				xElement.Add((
					from i in rootFolder.GetFoldersList()
					select this.CreateFolder(i)).Union<XElement>(
					from i in rootFolder.GetFilesList()
					select this.CreateItem(i)).Skip<XElement>(num1).Take<XElement>(num));
			}
			else
			{
				xElement.Add(this.CreateFolder(rootFolder));
			}
			yield return new XElement("Result", xElement.ToString());
			yield return new XElement("NumberReturned", (object)xElement.Elements().Count<XElement>());
			yield return new XElement("TotalMatches", (object)rootFolder.ItemsList.Count);
			yield return new XElement("UpdateID", (object)this.systemID);
			yield break;
		Label0:
			rootFolder = this.dataLibrary.GetItemById(new Guid(item), this.dataLibrary.RootFolder) as MultimediaFolder;
			goto Label2;
			goto Label0;
		}

		private IEnumerable<XElement> HandleGetSearchCapabilities()
		{
			yield return new XElement("SearchCaps", string.Empty);
		}

		private IEnumerable<XElement> HandleGetSortCapabilities()
		{
			yield return new XElement("SortCaps", string.Empty);
		}

		private IEnumerable<XElement> HandleGetSystemUpdateID()
		{
			yield return new XElement("Id", (object)this.systemID);
		}

		public HttpResponse HandleRequest(HttpRequest request)
		{
			IEnumerable<XElement> xElements;
			HttpResponse httpResponse;
			XElement xElement = XElement.Load(request.Body);
			XElement xElement1 = xElement.Element("{http://schemas.xmlsoap.org/soap/envelope/}Body").Elements().First<XElement>();
			XName name = xElement1.Name;
			Dictionary<string, string> dictionary = xElement1.Elements().ToDictionary<XElement, string, string>((XElement i) => i.Name.LocalName, (XElement i) => i.Value);
			try
			{
				string localName = name.LocalName;
				string str = localName;
				if (localName != null)
				{
					if (str == "GetSearchCapabilities")
					{
						xElements = this.HandleGetSearchCapabilities();
					}
					else if (str == "GetSortCapabilities")
					{
						xElements = this.HandleGetSortCapabilities();
					}
					else if (str == "GetSystemUpdateID")
					{
						xElements = this.HandleGetSystemUpdateID();
					}
					else if (str == "Browse")
					{
						xElements = this.HandleBrowse(request, dictionary);
					}
					else
					{
						if (str != "X_GetFeatureList")
						{
							throw new Exception();
						}
						xElements = this.HandleXGetFeatureList();
					}
					XDeclaration xDeclaration = new XDeclaration("1.0", "utf-8", "yes");
					object[] objArray = new object[1];
					XName xName = this.s + "Envelope";
					object[] xAttribute = new object[] { new XAttribute(XNamespace.Xmlns + "s", this.s), new XAttribute(this.s + "encodingStyle", "http://schemas.xmlsoap.org/soap/encoding/"), null };
					XName xName1 = this.s + "Body";
					XName xName2 = string.Concat(name.Namespace + name.LocalName, "Response");
					object[] xAttribute1 = new object[] { new XAttribute(XNamespace.Xmlns + "u", name.Namespace), xElements };
					xAttribute[2] = new XElement(xName1, new XElement(xName2, xAttribute1));
					objArray[0] = new XElement(xName, xAttribute);
					XDocument xDocument = new XDocument(xDeclaration, objArray);
					HttpResponse httpResponse1 = new HttpResponse(request, HttpCode.Ok, "text/xml", string.Concat(xDocument.Declaration.ToString(), xDocument.ToString()));
					httpResponse1.Headers.Add("EXT", string.Empty);
					httpResponse = httpResponse1;
					return httpResponse;
				}
				throw new Exception();
			}
			catch (Exception exception)
			{
				httpResponse = new HttpResponse(request, HttpCode.InternalError, "text/xml", Properties.Resources.error);
			}
			return httpResponse;
		}

		private IEnumerable<XElement> HandleXGetFeatureList()
		{
			yield return new XElement("FeatureList", Properties.Resources.x_featurelist);
		}
	}
}