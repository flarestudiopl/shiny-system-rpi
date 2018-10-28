using Commons.Extensions;
using Commons.Localization;
using Storage.StorageDatabase.User;

namespace HeatingControl.Application.Commands
{
    public class UpdateUserCommmand
    {
        public int UserId { get; set; }
        public string Password { get; set; }
        public string Pin { get; set; }
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
            if (command.Pin != null)
            {
                if (command.Pin.IsNullOrEmpty() || !command.Pin.ContainsDigitsOnly() || !command.Pin.HasLengthBetween(4, 8))
                {
                    return CommandResult.WithValidationError(Localization.ValidationMessage.IncorrectPinError);
                }
            }

            if (command.Password != null)
            {
                if (command.Password.IsNullOrEmpty())
                {
                    return CommandResult.WithValidationError(Localization.ValidationMessage.PasswordCannotBeEmpty);
                }
            }

            _userUpdater.Update(new UserUpdaterInput
            {
                UserId = command.UserId,
                PasswordHash = command.Password?.CalculateHash(),
                QuickLoginPinHash = command.Pin?.CalculateHash()
            });

            return CommandResult.Empty;
        }
    }
}
