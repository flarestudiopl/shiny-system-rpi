using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace HeatingApi.Controllers
{
    [Produces("application/json")]
    [Authorize]
    public class BaseController : Controller
    {
        internal int UserId
        {
            get
            {
                var idFromToken = Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type == @"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

                return idFromToken != null ? int.Parse(idFromToken.Value) : -1;
            }
        }
    }
}
