using Domain;
using HeatingControl.Application.DataAccess;
using System;

namespace HeatingControl.Application.Commands
{
    public class RemoveUserCommmand
    {
        public int UserId { get; set; }
    }

    public class RemoveUserCommandExecutor : ICommandExecutor<RemoveUserCommmand>
    {
        private readonly IRepository<User> _userRepository;

        public RemoveUserCommandExecutor(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public CommandResult Execute(RemoveUserCommmand command, CommandContext context)
        {
            var user = _userRepository.ReadSingleOrDefault(x => x.UserId == command.UserId &&
                                                                x.IsActive &&
                                                                x.IsBrowseable);

            if (user != null)
            {
                user.IsActive = false;
                user.DisabledByUserId = context.UserId;
                user.DisabledDate = DateTime.UtcNow;

                _userRepository.Update(user);
            }

            return CommandResult.Empty;
        }
    }
}
