﻿using HeatingControl.Models;

namespace HeatingControl.Application.Commands
{
    public interface ICommandExecutor<in TCommand>
    {
        CommandResult Execute(TCommand command, ControllerState controllerState);
    }

    public class CommandResult
    {
        public object Result { get; set; }
        public string ValidationError { get; set; }

        public static CommandResult WithValidationError(string validationError) => new CommandResult { ValidationError = validationError };
        public static CommandResult WithResult(object result) => new CommandResult { Result = result };
    }
}