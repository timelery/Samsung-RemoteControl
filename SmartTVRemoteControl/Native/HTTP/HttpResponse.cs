using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace SmartTVRemoteControl.Native.HTTP
{
	public class HttpResponse
	{
		public Stream Body
		{
			get;
			internal set;
		}

		public SmartTVRemoteControl.Native.HTTP.Headers Headers
		{
			get;
			internal set;
		}

		public HttpRequest Request
		{
			get;
			internal set;
		}

		public HttpCode Status
		{
			get;
			internal set;
		}

		public HttpResponse(HttpRequest request, HttpCode status, string type, Stream body)
		{
			this.Headers = new SmartTVRemoteControl.Native.HTTP.Headers(true);
			this.Request = request;
			this.Status = status;
			this.Body = body;
			this.Body.Seek((long)0, SeekOrigin.Begin);
			this.Headers["Server"] = HttpServer.Signature;
			this.Headers["Date"] = DateTime.Now.ToString("R");
			this.Headers["Connection"] = "keep-alive";
			this.Headers["Cache-Control"] = "no-cache";
			this.Headers["Content-Type"] = type;
			this.Headers["Content-Length"] = this.Body.Length.ToString();
		}

		public HttpResponse(HttpRequest request, HttpCode status, string type = "text/html", string body = "") : this(request, status, type, new MemoryStream(Encoding.UTF8.GetBytes(body)))
		{
		}
	}
}