using Commons.Extensions;
using Commons.Localization;
using Domain.StorageDatabase;
using HeatingControl.Application.DataAccess;

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
        private readonly IRepository<User> _userRepository;

        public UpdateUserCommandExecutor(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
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

            var user = _userRepository.Read(command.UserId);

            user.PasswordHash = command.Password?.CalculateHash() ?? user.PasswordHash;
            user.QuickLoginPinHash = command.Pin?.CalculateHash() ?? user.QuickLoginPinHash;

            _userRepository.Update(user);

            return CommandResult.Empty;
        }
    }
}
