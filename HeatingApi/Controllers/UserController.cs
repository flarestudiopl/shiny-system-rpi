using Commons.Extensions;
using HeatingControl.Application.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    [Route("/api/user")]
    public class UserController : BaseController
    {
        private readonly IAuthenticateUserExecutor _authenticateUserExecutor;

        public UserController(IAuthenticateUserExecutor authenticateUserExecutor)
        {
            _authenticateUserExecutor = authenticateUserExecutor;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult IssueToken(string login, string password)
        {
            var token = _authenticateUserExecutor.Execute(new AuthenticateUserExecutorInput
            {
                Login = login,
                Password = password,
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString()
            });

            if (token.IsNullOrEmpty())
            {
                return Unauthorized();
            }

            return Ok(new { token });
        }
    }
}
