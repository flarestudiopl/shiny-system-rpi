using Commons.Extensions;
using HeatingControl.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    [Produces("application/json")]
    [Route("/api/user")]
    public class UserController : Controller
    {
        private readonly IJwtTokenProvider _jwtTokenProvider;

        public UserController(IJwtTokenProvider jwtTokenProvider)
        {
            _jwtTokenProvider = jwtTokenProvider;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult IssueToken([FromBody] JwtTokenProviderInput input)
        {
            var token = _jwtTokenProvider.Provide(input);

            if (token.IsNullOrEmpty())
            {
                return Unauthorized();
            }

            return Ok(new { token });
        }
    }
}
