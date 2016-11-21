using System;

namespace SmartTVRemoteControl.Native.HTTP
{
	public interface IHttpHandler
	{
		string Prefix
		{
			get;
		}

		HttpResponse HandleRequest(HttpRequest request);
	}
}