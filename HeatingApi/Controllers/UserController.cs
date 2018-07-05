using HeatingControl.Application.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}
