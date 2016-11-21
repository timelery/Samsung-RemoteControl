using SmartView2.Core;
using SmartTVRemoteControl.Native.DLNA.Handler;
using SmartTVRemoteControl.Native.DLNA.SSDP;
using SmartTVRemoteControl.Native.HTTP;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SmartTVRemoteControl.Native.DLNA
{
	internal class DlnaServer : IDlnaServer, IDisposable
	{
		public readonly static Guid ServerGuid;

		private SsdpServer ssdpServer;

		private HttpServer httpServer;

		static DlnaServer()
		{
			DlnaServer.ServerGuid = Guid.NewGuid();
		}

		public DlnaServer()
		{
		}

		public void Close()
		{
			if (this.ssdpServer != null)
			{
				this.ssdpServer.UnregisterNotification(DlnaServer.ServerGuid);
			}
		}

		public void Dispose()
		{
			this.Close();
		}

		public Task InitializeAsync(string serverAddress, string deviceAddress, IDataLibrary dataLibrary, IHttpServer httpServer)
		{
            //Console.WriteLine("Initializing DLNA server");
			return Task.Factory.StartNew(() => {
				this.httpServer = httpServer as HttpServer;
				IPAddress pAddress = IPAddress.Parse(serverAddress);
				Uri uri = new Uri(string.Concat(httpServer.ServerUrl, "/description.xml"));
				this.ssdpServer = new SsdpServer();
				this.ssdpServer.RegisterNotification(DlnaServer.ServerGuid, uri, pAddress);
				this.httpServer.AddHandler(new DescriptionHandler());
				this.httpServer.AddHandler(new ContentDirectoryHandler());
				this.httpServer.AddHandler(new ControlHandler(httpServer, dataLibrary));
				this.httpServer.AddHandler(new FileHandler(dataLibrary));
			});
		}
	}
}