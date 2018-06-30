using Autofac;
using Commons;
using HeatingControl.Application.Commands;
using Microsoft.AspNetCore.Mvc;
using Commons.Extensions;

namespace HeatingApi
{
    public interface ICommandHandler
    {
        IActionResult ExecuteCommand<TCommand>(TCommand command);
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

        public IActionResult ExecuteCommand<TCommand>(TCommand command)
        {
            var commandExecutor = _componentContext.Resolve<ICommandExecutor<TCommand>>();

            CommandResult commandResult;

            try
            {
                commandResult = commandExecutor.Execute(command, _heatingControl.State);
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

            if (commandResult.Result == null)
            {
                return new OkResult();
            }

            return new OkObjectResult(commandResult.Result);
        }
    }
}
