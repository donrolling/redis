using Microsoft.Extensions.Logging;

namespace Common.Extensions
{
    public static class LoggerHelper
    {
        public static void LogInformation(this ILogger logger, object obj, string message)
        {
            var objString = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var completeMessage = $"{ message } | Object:\r\n{ objString }";
            logger.LogInformation(completeMessage);
        }
    }
}