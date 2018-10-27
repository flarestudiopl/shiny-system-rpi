using Commons.Extensions;
using Storage.StorageDatabase.User;

namespace HeatingControl.Application.Commands
{
    public class UpdateUserCommmand
    {
        public int UserId { get; set; }
        public string Password { get; set; }
        public string QuickLoginPin { get; set; }
    }

    public class UpdateUserCommandExecutor : ICommandExecutor<UpdateUserCommmand>
    {
        private readonly IUserUpdater _userUpdater;

        public UpdateUserCommandExecutor(IUserUpdater userUpdater)
        {
            _userUpdater = userUpdater;
        }

        public CommandResult Execute(UpdateUserCommmand command, CommandContext context)
        {
            _userUpdater.Update(new UserUpdaterInput
            {
                UserId = command.UserId,
                PasswordHash = command.Password?.CalculateHash(),
                QuickLoginPinHash = command.QuickLoginPin?.CalculateHash()
            });

            return CommandResult.Empty;
        }
    }
}
