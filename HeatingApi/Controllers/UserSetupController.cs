using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    /// <summary>
    /// Controller for settings views
    /// </summary>
    [Route("/api/setup/user")]
    public class UserSetupController : BaseController
    {
        private readonly IUserListProvider _userListProvider;
        private readonly IUserProvider _userProvider;
        private readonly ICommandHandler _commandHandler;

        public UserSetupController(IUserListProvider userListProvider, IUserProvider userProvider,
                                   ICommandHandler commandHandler)
        {
            _userListProvider = userListProvider;
            _userProvider = userProvider;
            _commandHandler = commandHandler;
        }

        [HttpGet]
        public UserListProviderResult GetUserList()
        {
            return _userListProvider.Provide();
        }

        [HttpGet("{userId}")]
        public UserProviderResult GetUser(int userId)
        {
            return _userProvider.Provide(userId);
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] NewUserCommand command)
        {
            return _commandHandler.ExecuteCommand(command, UserId);
        }

        [HttpPut("{userId}")]
        public IActionResult UpdateUser(int userId, [FromBody] UpdateUserCommmand command)
        {
            command.UserId = userId;
            return _commandHandler.ExecuteCommand(command, UserId);
        }

        [HttpDelete("{userId}")]
        public IActionResult RemoveUser(int userId)
        {
            return _commandHandler.ExecuteCommand(new RemoveUserCommmand
                                                  {
                                                      UserId = userId
                                                  },
                                                  UserId);
        }
    }
}
