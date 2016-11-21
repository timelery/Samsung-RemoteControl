using System;
using System.Collections.Generic;

namespace SmartTVRemoteControl.Native.HTTP
{
	internal class HttpPhrases
	{
		public readonly static IDictionary<HttpCode, string> Phrases;

		static HttpPhrases()
		{
			Dictionary<HttpCode, string> httpCodes = new Dictionary<HttpCode, string>()
			{
				{ HttpCode.Ok, "OK" },
				{ HttpCode.Partial, "Partial Content" },
				{ HttpCode.MovedPermanently, "Moved Permanently" },
				{ HttpCode.NotModified, "Not Modified" },
				{ HttpCode.TemporaryRedirect, "Temprary Redirect" },
				{ HttpCode.Denied, "Forbidden" },
				{ HttpCode.NotFound, "Not Found" },
				{ HttpCode.RangeNotSatisfiable, "Requested Range not satisfiable" },
				{ HttpCode.InternalError, "Internal Server Error" }
			};
			HttpPhrases.Phrases = httpCodes;
		}

		public HttpPhrases()
		{
		}
	}
}