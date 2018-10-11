using HeatingControl.Application.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace HeatingApi.Controllers
{
    [Route("/api/user")]
    public class UserController : BaseController
    {
        private readonly ICommandHandler _commandHandler;

        public UserController(ICommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
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
        public IActionResult IssueTokenByPin(string login, int pin)
        {
            throw new NotImplementedException();
        }

        [AllowAnonymous]
        [HttpGet("list")]
        public IActionResult ListPinAllowedUsers()
        {
            if (!IPAddress.IsLoopback(Request.HttpContext.Connection.RemoteIpAddress))
            {
                return new BadRequestResult();
            }

            // TODO - users that can log in via pin

            return new OkResult();
        }
        
    }
}
