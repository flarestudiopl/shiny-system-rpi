using Autofac;
using Commons;
using HeatingControl.Application.Commands;
using Microsoft.AspNetCore.Mvc;
using Commons.Extensions;

namespace HeatingApi
{
    public interface ICommandHandler
    {
        IActionResult ExecuteCommand<TCommand>(TCommand command, int userId);
    }

    public class CommandHandler : ICommandHandler
    {
        private readonly IComponentContext _componentContext;
        private readonly IHeatingControl _heatingControl;

        public CommandHandler(IComponentContext componentContext,
                              IHeatingControl heatingControl)
        {
            _componentContext = componentContext;
            _heatingControl = heatingControl;
        }

        public IActionResult ExecuteCommand<TCommand>(TCommand command, int userId)
        {
            var commandExecutor = _componentContext.Resolve<ICommandExecutor<TCommand>>();

            CommandResult commandResult;

            try
            {
                commandResult = commandExecutor.Execute(command, new CommandContext
                                                                 {
                                                                     ControllerState = _heatingControl.State,
                                                                     UserId = userId
                                                                 });
            }
            catch
            {
                Logger.TraceWithData("Command that raised the exception", command);

                throw;
            }

            if (!commandResult.ValidationError.IsNullOrEmpty())
            {
                Logger.TraceWithData(commandResult.ValidationError, command);

                return new BadRequestObjectResult(commandResult.ValidationError);
            }

            if (commandResult.Response == null)
            {
                return new OkResult();
            }

            return new OkObjectResult(commandResult.Response);
        }
    }
}
