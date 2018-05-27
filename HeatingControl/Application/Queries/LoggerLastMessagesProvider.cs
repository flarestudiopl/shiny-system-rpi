using System.Collections.Generic;
using Commons;

namespace HeatingControl.Application.Queries
{
    public interface ILoggerLastMessagesProvider
    {
        ICollection<Logger.LoggerMessage> Provide();
    }

    public class LoggerLastMessagesProvider : ILoggerLastMessagesProvider
    {
        public ICollection<Logger.LoggerMessage> Provide()
        {
            return Logger.LastMessages.ToArray();
        }
    }
}
