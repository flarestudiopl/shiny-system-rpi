using Commons.Extensions;
using Commons.Localization;
using Storage.StorageDatabase.User;

namespace HeatingControl.Application.Commands
{
    public class NewUserCommand
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class NewUserCommandExecutor : ICommandExecutor<NewUserCommand>
    {
        private readonly IActiveUserByLoginProvider _activeUserByLoginProvider;
        private readonly IUserSaver _userSaver;

        public NewUserCommandExecutor(IActiveUserByLoginProvider activeUserByLoginProvider,
                               IUserSaver userSaver)
        {
            _activeUserByLoginProvider = activeUserByLoginProvider;
            _userSaver = userSaver;
        }

        public CommandResult Execute(NewUserCommand command, CommandContext context)
        {
            if (_activeUserByLoginProvider.Provide(command.Login) != null)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UserAlreadyExists.FormatWith(command.Login));
            }

            if (command.Login.IsNullOrEmpty() || command.Password.IsNullOrEmpty())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UserNameAndLoginShallNotBeEmpty);
            }

            _userSaver.Save(command.Login, command.Password.CalculateHash(), context.UserId);

            return CommandResult.Empty;
        }
    }
}
