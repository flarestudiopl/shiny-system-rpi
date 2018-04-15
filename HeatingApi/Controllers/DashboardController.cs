using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatingApi.Controllers
{
    [Produces("application/json")]
    [Route("/api/dashboard")]
    public class DashboardController : Controller
    {
        [HttpGet]
        public void GetSnapshot()
        {

        }
    }
}
