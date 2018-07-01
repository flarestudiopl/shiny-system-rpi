using HeatingControl.Application.Loops.Processing;

namespace HeatingControl.Application.Commands
{
    public class DisableAllOutputsCommand
    {
    }

    public class DisableAllOutputsCommandExecutor : ICommandExecutor<DisableAllOutputsCommand>
    {
        private readonly IOutputsWriter _outputsWriter;

        public DisableAllOutputsCommandExecutor(IOutputsWriter outputsWriter)
        {
            _outputsWriter = outputsWriter;
        }

        public CommandResult Execute(DisableAllOutputsCommand command, CommandContext context)
        {
            foreach (var heaterToState in context.ControllerState.HeaterIdToState)
            {
                heaterToState.Value.OutputState = false;
            }

            _outputsWriter.Write(context.ControllerState, true);

            return CommandResult.Empty;
        }
    }
}
