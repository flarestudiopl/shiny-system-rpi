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
        public ICollection<Logger.LoggerMessage> Messages { get; set; }
    }

    public class LoggerLastMessagesProvider : ILoggerLastMessagesProvider
    {
        public LoggerLastMessagesProviderResult Provide()
        {
            return new LoggerLastMessagesProviderResult
                   {
                       Messages = Logger.LastMessages.ToArray()
                   };
        }
    }
}
