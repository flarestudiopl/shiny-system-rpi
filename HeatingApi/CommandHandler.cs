using System;
using Autofac;
using Commons;
using HeatingControl.Application.Commands;
using Microsoft.AspNetCore.Mvc;
using Commons.Extensions;

namespace HeatingApi
{
    public interface ICommandHandler
    {
        IActionResult ExecuteCommand<TCommand>(TCommand command, int userId, Func<object, IActionResult> responseTransform = null);
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

        public IActionResult ExecuteCommand<TCommand>(TCommand command, int userId, Func<object, IActionResult> responseTransform = null)
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
                Logger.DebugWithData("Command that raised the exception", command);

                throw;
            }

            if (!commandResult.ValidationError.IsNullOrEmpty())
            {
                Logger.DebugWithData(commandResult.ValidationError, command);

                return new BadRequestObjectResult(commandResult.ValidationError);
            }

            if (commandResult.Response == null)
            {
                return new OkResult();
            }

            return responseTransform != null
                       ? responseTransform(commandResult.Response)
                       : new OkObjectResult(commandResult.Response);
        }
    }
}
