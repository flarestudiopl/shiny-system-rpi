using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace HeatingApi.Controllers
{
    [Produces("application/json")]
    [Authorize]
    public class BaseController : Controller
    {
        internal int UserId => int.Parse(Request.HttpContext.User.Claims.First(x => x.Type == @"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value);
    }
}
