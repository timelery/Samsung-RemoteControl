using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace SmartTVRemoteControl.Native.HTTP
{
	public class HttpRequest
	{
		public Stream Body
		{
			get;
			internal set;
		}

		public TcpClient Client
		{
			get;
			private set;
		}

		public SmartTVRemoteControl.Native.HTTP.Headers Headers
		{
			get;
			internal set;
		}

		public string Method
		{
			get;
			internal set;
		}

		public string Path
		{
			get;
			internal set;
		}

		public HttpRequest(TcpClient client)
		{
			this.Method = null;
			this.Path = null;
			this.Headers = new SmartTVRemoteControl.Native.HTTP.Headers();
			this.Body = new MemoryStream();
			this.Client = client;
		}
	}
}