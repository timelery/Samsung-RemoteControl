using log4net;
using SmartView2.Core;
using System;
using System.Reflection;

namespace SmartTVRemoteControl
{
	public class Log4NetLogger : Logger
	{
		private readonly static ILog logger;

		static Log4NetLogger()
		{
			Log4NetLogger.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public Log4NetLogger()
		{
		}

		public override void LogError(string message)
		{
		}

		public override void LogMessage(string message)
		{
		}
	}
}