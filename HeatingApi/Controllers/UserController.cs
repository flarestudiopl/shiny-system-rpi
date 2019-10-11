using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HeatingApi.Controllers
{
    [Route("/api/user")]
    public class UserController : BaseController
    {
        private readonly ICommandHandler _commandHandler;
        private readonly IPinAllowedUserListProvider _pinAllowedUserListProvider;

        public UserController(ICommandHandler commandHandler,
                              IPinAllowedUserListProvider pinAllowedUserListProvider)
        {
            _commandHandler = commandHandler;
            _pinAllowedUserListProvider = pinAllowedUserListProvider;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult IssueToken([FromBody] AuthenticateUserParams authenticateUserParams)
        {
            var command = new AuthenticateUserCommand
                          {
                              Login = authenticateUserParams.Login,
                              Password = authenticateUserParams.Password,
                              Pin = authenticateUserParams.Pin,
                              IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString()
                          };

            return _commandHandler.ExecuteCommand(command, -1);
        }

        [AllowAnonymous]
        [HttpGet("list")]
        public PinAllowedUserListProviderResult ListPinAllowedUsers()
        {
            // TODO - list of allowed hosts stored in configuration
            if (!IPAddress.IsLoopback(Request.HttpContext.Connection.RemoteIpAddress))
            {
                return null;
            }

           return _pinAllowedUserListProvider.Provide();
        }

        public class AuthenticateUserParams
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string Pin { get; set; }
        }
    }
}
