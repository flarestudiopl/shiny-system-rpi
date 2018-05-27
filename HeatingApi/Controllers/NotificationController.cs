using System.Collections.Generic;
using Commons;
using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    [Route("/api/notification")]
    public class NotificationController : BaseController
    {
        private readonly ILoggerLastMessagesProvider _loggerLastMessagesProvider;
        private readonly IClearLoggerLastMessagesExecutor _clearLoggerLastMessagesExecutor;

        public NotificationController(ILoggerLastMessagesProvider loggerLastMessagesProvider,
                                      IClearLoggerLastMessagesExecutor clearLoggerLastMessagesExecutor)
        {
            _loggerLastMessagesProvider = loggerLastMessagesProvider;
            _clearLoggerLastMessagesExecutor = clearLoggerLastMessagesExecutor;
        }

        [HttpGet]
        public ICollection<Logger.LoggerMessage> LastAlerts()
        {
            return _loggerLastMessagesProvider.Provide();
        }

        [HttpDelete]
        public void ClearAlerts()
        {
            _clearLoggerLastMessagesExecutor.Execute();
        }
    }
}
