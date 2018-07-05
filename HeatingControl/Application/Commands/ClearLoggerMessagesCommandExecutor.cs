using Commons;

namespace HeatingControl.Application.Commands
{
    public class ClearLoggerMessagesCommand
    {
    }

    public class ClearLoggerMessagesCommandExecutor : ICommandExecutor<ClearLoggerMessagesCommand>
    {
        public CommandResult Execute(ClearLoggerMessagesCommand command, CommandContext context)
        {
            Logger.LastMessages.Clear();

            return CommandResult.Empty;
        }
    }
}
