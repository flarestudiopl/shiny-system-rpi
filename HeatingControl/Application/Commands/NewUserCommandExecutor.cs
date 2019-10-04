using Commons.Extensions;
using Commons.Localization;
using Domain;
using HeatingControl.Application.DataAccess;
using System;

namespace HeatingControl.Application.Commands
{
    public class NewUserCommand
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Pin { get; set; }
    }

    public class NewUserCommandExecutor : ICommandExecutor<NewUserCommand>
    {
        private readonly IRepository<User> _userRepository;

        public NewUserCommandExecutor(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public CommandResult Execute(NewUserCommand command, CommandContext context)
        {
            if (_userRepository.ReadSingleOrDefault(x => x.IsActive &&
                                                         x.IsBrowseable &&
                                                         x.Login == command.Login) != null)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UserAlreadyExists.FormatWith(command.Login));
            }

            if (command.Login.IsNullOrEmpty() || command.Password.IsNullOrEmpty())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UserNameAndLoginShallNotBeEmpty);
            }

            if (command.Pin != null)
            {
                if (command.Pin.IsNullOrEmpty() || !command.Pin.ContainsDigitsOnly() || !command.Pin.HasLengthBetween(4, 8))
                {
                    return CommandResult.WithValidationError(Localization.ValidationMessage.IncorrectPinError);
                }
            }

            var user = new User
            {
                Login = command.Login,
                PasswordHash = command.Password.CalculateHash(),
                QuickLoginPinHash = command.Pin?.CalculateHash(),
                CreatedByUserId = context.UserId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                IsBrowseable = true
            };

            _userRepository.Create(user, null);

            return CommandResult.Empty;
        }
    }
}
