﻿using HeatingControl.Models;

namespace HeatingControl.Application.Commands
{
    public interface ICommandExecutor<in TCommand>
    {
        CommandResult Execute(TCommand command, CommandContext context);
    }

    public class CommandContext
    {
        public ControllerState ControllerState { get; set; }
        public int UserId { get; set; }
    }

    public class CommandResult
    {
        public object Response { get; set; }
        public string ValidationError { get; set; }

        public static CommandResult WithValidationError(string validationError) => new CommandResult { ValidationError = validationError };
        public static CommandResult WithResponse(object response) => new CommandResult { Response = response };
        public static CommandResult Empty => new CommandResult();
    }
}
