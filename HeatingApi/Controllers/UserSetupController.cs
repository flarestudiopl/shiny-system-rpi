using Domain;
using HeatingApi.Attributes;
using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    /// <summary>
    /// Controller for settings views
    /// </summary>
    [Route("/api/setup/user")]
    [RequiredPermission(Permission.Configuration_Users)]
    public class UserSetupController : BaseController
    {
        private readonly IUserListProvider _userListProvider;
        private readonly IPermissionsProvider _permissionsProvider;
        private readonly IUserProvider _userProvider;
        private readonly ICommandHandler _commandHandler;

        public UserSetupController(IUserListProvider userListProvider,
                                   IPermissionsProvider permissionsProvider,
                                   IUserProvider userProvider,
                                   ICommandHandler commandHandler)
        {
            _userListProvider = userListProvider;
            _permissionsProvider = permissionsProvider;
            _userProvider = userProvider;
            _commandHandler = commandHandler;
        }

        [HttpGet]
        public UserListProviderResult GetUserList()
        {
            return _userListProvider.Provide();
        }

        [HttpGet("permissions")]
        public PermissionsProviderResult GetPermissions()
        {
            return _permissionsProvider.Provide();
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
