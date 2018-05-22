using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    /// <summary>
    /// Controller for settings views
    /// </summary>
    [Produces("application/json")]
    [Route("/api/setup/user")]
    public class UserSetupController : Controller
    {
        private readonly IUserListProvider _userListProvider;
        private readonly INewUserExecutor _newUserExecutor;
        private readonly IRemoveUserExecutor _removeUserExecutor;

        public UserSetupController(IUserListProvider userListProvider,
                                   INewUserExecutor newUserExecutor,
                                   IRemoveUserExecutor removeUserExecutor)
        {
            _userListProvider = userListProvider;
            _newUserExecutor = newUserExecutor;
            _removeUserExecutor = removeUserExecutor;
        }

        [HttpGet]
        public UserListProviderResult GetUserList()
        {
            return _userListProvider.Provide();
        }

        [HttpPost]
        public void AddUser([FromBody] NewUserExecutorInput input)
        {
            _newUserExecutor.Execute(input, /* TODO */ -1);
        }

        [HttpDelete("{userId}")]
        public void DeleteUser(int userId)
        {
            _removeUserExecutor.Execute(userId, /* TODO */ -1);
        }
    }
}
