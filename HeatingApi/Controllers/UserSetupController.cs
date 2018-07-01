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
        private readonly ICommandHandler _commandHandler;

        public UserSetupController(IUserListProvider userListProvider,
                                   ICommandHandler commandHandler)
        {
            _userListProvider = userListProvider;
            _commandHandler = commandHandler;
        }

        [HttpGet]
        public UserListProviderResult GetUserList()
        {
            return _userListProvider.Provide();
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] NewUserCommand command)
        {
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
