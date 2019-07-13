using System;
using System.Collections.Generic;
using Commons;

namespace HeatingControl.Application.Queries
{
    public interface ILoggerLastMessagesProvider
    {
        LoggerLastMessagesProviderResult Provide();
    }

    public class LoggerLastMessagesProviderResult
    {
        public DateTime ControllerDateTime { get; set; }
        public ICollection<Logger.LoggerMessage> Messages { get; set; }
    }

    public class LoggerLastMessagesProvider : ILoggerLastMessagesProvider
    {
        public LoggerLastMessagesProviderResult Provide()
        {
            return new LoggerLastMessagesProviderResult
                   {
                       ControllerDateTime = DateTime.UtcNow,
                       Messages = Logger.LastMessages.ToArray()
                   };
        }
    }
}
