using Commons.Extensions;
using HeatingControl.Application.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    [Route("/api/user")]
    public class UserController : BaseController
    {
        private readonly ICommandExecutor<AuthenticateUserCommand> _authenticateUserCommandExecutor;

        public UserController(ICommandExecutor<AuthenticateUserCommand> authenticateUserCommandExecutor)
        {
            _authenticateUserCommandExecutor = authenticateUserCommandExecutor;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult IssueToken(string login, string password)
        {
            var commandResult = _authenticateUserCommandExecutor.Execute(new AuthenticateUserCommand
                                                                         {
                                                                             Login = login,
                                                                             Password = password,
                                                                             IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString()
                                                                         },
                                                                         null);

            if (!commandResult.ValidationError.IsNullOrEmpty())
            {
                return BadRequest(commandResult.ValidationError);
            }

            if (commandResult.Response == null)
            {
                return Unauthorized();
            }

            return Ok(new
                      {
                          token = commandResult.Response
                      });
        }
    }
}
