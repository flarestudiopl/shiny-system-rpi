using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    [Route("/api/notification")]
    public class NotificationController : BaseController
    {
        private readonly ILoggerLastMessagesProvider _loggerLastMessagesProvider;
        private readonly ICommandHandler _commandHandler;

        public NotificationController(ILoggerLastMessagesProvider loggerLastMessagesProvider,
                                      ICommandHandler commandHandler)
        {
            _loggerLastMessagesProvider = loggerLastMessagesProvider;
            _commandHandler = commandHandler;
        }

        [HttpGet]
        public LoggerLastMessagesProviderResult LastAlerts()
        {
            return _loggerLastMessagesProvider.Provide();
        }

        [HttpDelete]
        public IActionResult ClearAlerts()
        {
            return _commandHandler.ExecuteCommand(new ClearLoggerMessagesCommand(), UserId);
        }
    }
}
