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
        public IActionResult IssueToken(string login, string password)
        {
            var command = new AuthenticateUserCommand
                          {
                              Login = login,
                              Password = password,
                              IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString()
                          };

            return _commandHandler.ExecuteCommand(command, -1, result => Ok(new
                                                                            {
                                                                                token = result
                                                                            }));
        }

        [AllowAnonymous]
        [HttpPost("authenticate-by-pin")]
        public IActionResult IssueTokenByPin(string login, string pin)
        {
            // TODO - list of allowed hosts stored in configuration
            if (!IPAddress.IsLoopback(Request.HttpContext.Connection.RemoteIpAddress))
            {
                return null;
            }

            var command = new AuthenticateUserByPinCommand
                          {
                              Login = login,
                              Pin = pin,
                              IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString()
                          };

            return _commandHandler.ExecuteCommand(command, -1, result => Ok(new
                                                                            {
                                                                                token = result
                                                                            }));
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
    }
}
