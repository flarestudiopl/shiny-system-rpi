using Storage.StorageDatabase.User;

namespace HeatingControl.Application.Commands
{
    public class RemoveUserCommmand
    {
        public int UserId { get; set; }
    }

    public class RemoveUserCommandExecutor : ICommandExecutor<RemoveUserCommmand>
    {
        private readonly IUserDeactivator _userDeactivator;

        public RemoveUserCommandExecutor(IUserDeactivator userDeactivator)
        {
            _userDeactivator = userDeactivator;
        }

        public CommandResult Execute(RemoveUserCommmand command, CommandContext context)
        {
            _userDeactivator.Deactivate(command.UserId, context.UserId);

            return CommandResult.Empty;
        }
    }
}
