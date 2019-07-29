using Common.Logging;
using Microsoft.Extensions.Logging;

namespace Common.BaseClasses
{
	public abstract class LoggingWorker
	{
		public ILogger Logger { get; }

        protected LoggingWorker(ILoggerFactory loggerFactory)
		{
			this.Logger = LogUtility.GetLogger(loggerFactory, this.GetType());
		}
	}
}